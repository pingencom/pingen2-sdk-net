using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Emails;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Tests.Helpers;

public class PingenSerialisationHelperTests
{
    [Test]
    public void Serialize_SimpleObject_ReturnsValidJson()
    {
        var obj = new { name = "test", value = 42 };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldContain("\"name\":\"test\"");
        json.ShouldContain("\"value\":42");
    }

    [Test]
    public void Deserialize_ValidJson_ReturnsObject()
    {
        string json = "{\"id\":\"abc-123\",\"type\":\"letters\"}";

        DataIdentity? result = PingenSerialisationHelper.Deserialize<DataIdentity>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(() => result.Id.ShouldBe("abc-123"), () => result.Type.ShouldBe("letters"));
    }

    [Test]
    public async Task DeserializeAsync_ValidStream_ReturnsObject()
    {
        string json = "{\"id\":\"def-456\",\"type\":\"batches\"}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        DataIdentity? result = await PingenSerialisationHelper.DeserializeAsync<DataIdentity>(stream);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(() => result.Id.ShouldBe("def-456"), () => result.Type.ShouldBe("batches"));
    }

    [Test]
    public void Serialize_NullProperty_IsOmitted()
    {
        var obj = new { name = "test", extra = (string?)null };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldNotContain("extra");
    }

    [Test]
    public void PingenApiDataTypeMapping_ContainsExpectedTypes()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(
            () => mapping.ContainsKey("letters").ShouldBeTrue(),
            () => mapping.ContainsKey("batches").ShouldBeTrue(),
            () => mapping.ContainsKey("organisations").ShouldBeTrue(),
            () => mapping.ContainsKey("webhooks").ShouldBeTrue(),
            () => mapping.ContainsKey("users").ShouldBeTrue(),
            () => mapping.ContainsKey("file_uploads").ShouldBeTrue()
        );
    }

    [Test]
    public void TryGetIncludedData_MatchingType_ReturnsTrueWithData()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } },
                    { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        bool found = PingenSerialisationHelper.TryGetIncludedData<Organisation>(result, out Data<Organisation>? included);

        found.ShouldBeTrue();
        included.ShouldNotBeNull();
        included!.Id.ShouldBe("org-1");
        included.Type.ShouldBe("organisations");
    }

    [Test]
    public void TryGetIncludedData_NoMatchingType_ReturnsFalse()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        bool found = PingenSerialisationHelper.TryGetIncludedData<Organisation>(result, out Data<Organisation>? included);

        found.ShouldBeFalse();
        included.ShouldBeNull();
    }

    [Test]
    public void TryGetIncludedData_NullIncluded_ReturnsFalse()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        bool found = PingenSerialisationHelper.TryGetIncludedData<Organisation>(result, out Data<Organisation>? included);

        found.ShouldBeFalse();
        included.ShouldBeNull();
    }

    [Test]
    public void TryGetIncludedData_UnknownTypeDiscriminator_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "unknown-1", "type": "unknown_type_xyz", "attributes": {} },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        bool found = PingenSerialisationHelper.TryGetIncludedData<Organisation>(result, out Data<Organisation>? included);

        found.ShouldBeTrue();
        included.ShouldNotBeNull();
        included!.Id.ShouldBe("org-1");
    }

    [Test]
    public void Deserialize_EmptyJsonObject_ThrowsForRequiredProperties()
    {
        string json = "{}";

        Should.Throw<JsonException>(() => PingenSerialisationHelper.Deserialize<DataIdentity>(json));
    }

    [Test]
    public void Deserialize_MissingOptionalProperties_FillsWithNull()
    {
        string json = "{\"id\":\"x\"}";

        PartialModel? result = PingenSerialisationHelper.Deserialize<PartialModel>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(() => result.Id.ShouldBe("x"), () => result.Name.ShouldBeNull());
    }

    [Test]
    public void Deserialize_ExtraProperties_AreIgnored()
    {
        string json = "{\"id\":\"abc-123\",\"type\":\"letters\",\"unknown_field\":\"skipped\",\"another\":42}";

        DataIdentity? result = PingenSerialisationHelper.Deserialize<DataIdentity>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(() => result.Id.ShouldBe("abc-123"), () => result.Type.ShouldBe("letters"));
    }

    [Test]
    public void Deserialize_NestedObject_PreservesShape()
    {
        var inner = new InnerHolder { Date = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc) };
        var outer = new OuterHolder { Inner = inner };

        string json = PingenSerialisationHelper.Serialize(outer);
        OuterHolder? result = PingenSerialisationHelper.Deserialize<OuterHolder>(json);

        result.ShouldNotBeNull();
        result!.Inner.ShouldNotBeNull();
        result.Inner!.ShouldSatisfyAllConditions(
            () => result.Inner.Date.Year.ShouldBe(2024),
            () => result.Inner.Date.Month.ShouldBe(5),
            () => result.Inner.Date.Day.ShouldBe(6)
        );
    }

    [Test]
    public void Serialize_RecordWithCustomConverters_RoundTripsAllThree()
    {
        var holder = new TripleConverterHolder
        {
            Date = new DateTime(2024, 8, 1, 9, 30, 0, DateTimeKind.Utc),
            OptionalDate = new DateTime(2024, 8, 2, 10, 0, 0, DateTimeKind.Utc),
            Filter = new KeyValuePair<string, object>("name", "Jane"),
        };

        string json = PingenSerialisationHelper.Serialize(holder);

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("2024-08-01T09:30:00"),
            () => json.ShouldContain("2024-08-02T10:00:00"),
            () => json.ShouldContain("\"name\":\"Jane\"")
        );
    }

    [Test]
    public async Task DeserializeAsync_EmptyStream_Throws()
    {
        using var stream = new MemoryStream();

        await Should.ThrowAsync<JsonException>(async () => await PingenSerialisationHelper.DeserializeAsync<DataIdentity>(stream));
    }

    [Test]
    public void PingenApiDataTypeMapping_HasExpectedCount()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.Count.ShouldBe(18);
    }

    [Test]
    public void PingenApiDataTypeMapping_AllWebhookCategoriesMapToWebhookEvent()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(
            () => mapping["webhook_issues"].ShouldBe(typeof(WebhookEvent)),
            () => mapping["webhook_sent"].ShouldBe(typeof(WebhookEvent)),
            () => mapping["webhook_undeliverable"].ShouldBe(typeof(WebhookEvent)),
            () => mapping["webhook_delivered"].ShouldBe(typeof(WebhookEvent))
        );
    }

    [Test]
    public void PingenApiDataTypeMapping_PresetsNotMapped()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ContainsKey("presets").ShouldBeFalse();
    }

    [Test]
    public void PingenApiDataTypeMapping_BothEventDiscriminatorsMapToLetterEvent()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(
            () => mapping["letters_events"].ShouldBe(typeof(LetterEvent)),
            () => mapping["deliverables_events"].ShouldBe(typeof(LetterEvent))
        );
    }

    [Test]
    public void PingenApiDataTypeMapping_OtherDeliverableKindsMapped()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(() => mapping["emails"].ShouldBe(typeof(Email)), () => mapping["ebills"].ShouldBe(typeof(Ebill)));
    }

    [Test]
    public void TryGetIncludedData_MultipleMatches_Throws()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "First Org" } },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Second Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Should.Throw<InvalidOperationException>(() => PingenSerialisationHelper.TryGetIncludedData<Organisation>(result, out _));
    }

    private sealed record PartialModel
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }
    }

    private sealed record InnerHolder
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; init; }
    }

    private sealed record OuterHolder
    {
        [JsonPropertyName("inner")]
        public InnerHolder? Inner { get; init; }
    }

    private sealed record TripleConverterHolder
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; init; }

        [JsonPropertyName("optional_date")]
        public DateTime? OptionalDate { get; init; }

        [JsonPropertyName("filter")]
        public KeyValuePair<string, object> Filter { get; init; }
    }
}

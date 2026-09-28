using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;

namespace Pingen.Tests.Helpers;

public class PingenKeyValuePairStringObjectConverterTests
{
    [Test]
    public void Write_StringValue_SerializesAsJsonObject()
    {
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("name", "John") };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        filter.GetProperty("name").GetString().ShouldBe("John");
    }

    [Test]
    public void Write_NumericValue_SerializesNumber()
    {
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("count", 42) };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        filter.GetProperty("count").GetInt32().ShouldBe(42);
    }

    [Test]
    public void Write_BooleanValue_SerializesBoolean()
    {
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("active", true) };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        filter.GetProperty("active").GetBoolean().ShouldBeTrue();
    }

    [Test]
    public void Write_NestedObject_SerializesNestedStructure()
    {
        var nested = new Dictionary<string, object> { ["city"] = "Zurich" };
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("address", nested) };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        JsonElement address = filter.GetProperty("address");
        address.GetProperty("city").GetString().ShouldBe("Zurich");
    }

    [Test]
    public void Write_NullValue_SerializesKeyWithNullValue()
    {
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("key", null!) };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        filter.GetProperty("key").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    [Test]
    public void Write_EmptyStringKey_SerializesWithEmptyKey()
    {
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("", "value") };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filter = doc.RootElement.GetProperty("filter");
        filter.GetProperty("").GetString().ShouldBe("value");
    }

    [Test]
    public void Read_ValidJsonString_ReturnsKeyValuePair()
    {
        string json = "{\"filter\":\"{\\\"name\\\":\\\"John\\\"}\"}";

        KvpHolder? result = PingenSerialisationHelper.Deserialize<KvpHolder>(json);

        result.ShouldNotBeNull();
        result!.Filter.Key.ShouldBe("name");
        JsonElement valueElement = result.Filter.Value.ShouldBeOfType<JsonElement>();
        valueElement.GetString().ShouldBe("John");
    }

    [Test]
    public void Read_EmptyString_ReturnsDefault()
    {
        string json = "{\"filter\":\"\"}";

        KvpHolder? result = PingenSerialisationHelper.Deserialize<KvpHolder>(json);

        result.ShouldNotBeNull();
        result!.Filter.Key.ShouldBeNull();
        result.Filter.Value.ShouldBeNull();
    }

    [Test]
    public void Write_ListOfFilterExpressions_SerializesEachAsJsonObject()
    {
        var obj = new KvpListHolder
        {
            Filters =
            [
                new KeyValuePair<string, object>("name", "John"),
                new KeyValuePair<string, object>("status", "active"),
                new KeyValuePair<string, object>("count", 42),
            ],
        };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement filters = doc.RootElement.GetProperty("filters");
        filters.ValueKind.ShouldBe(JsonValueKind.Array);
        filters.GetArrayLength().ShouldBe(3);
        filters.ShouldSatisfyAllConditions(
            () => filters[0].GetProperty("name").GetString().ShouldBe("John"),
            () => filters[1].GetProperty("status").GetString().ShouldBe("active"),
            () => filters[2].GetProperty("count").GetInt32().ShouldBe(42)
        );
    }

    [Test]
    public void Write_EmptyListOfFilterExpressions_SerializesAsEmptyArray()
    {
        var obj = new KvpListHolder { Filters = [] };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldContain("\"filters\":[]");
    }

    [Test]
    public void Write_NullListOfFilterExpressions_OmitsProperty()
    {
        var obj = new KvpListHolder { Filters = null };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldNotContain("filters");
    }

    [Test]
    public void Write_ComplexFilterExpression_SerializesNested()
    {
        var innerEq = new Dictionary<string, object> { ["city"] = "Zurich", ["postcode"] = "8000" };
        var inner = new Dictionary<string, object> { ["eq"] = innerEq };
        var obj = new KvpHolder { Filter = new KeyValuePair<string, object>("address", inner) };

        string json = PingenSerialisationHelper.Serialize(obj);

        using var doc = JsonDocument.Parse(json);
        JsonElement address = doc.RootElement.GetProperty("filter").GetProperty("address");
        JsonElement eq = address.GetProperty("eq");
        eq.ShouldSatisfyAllConditions(
            () => eq.GetProperty("city").GetString().ShouldBe("Zurich"),
            () => eq.GetProperty("postcode").GetString().ShouldBe("8000")
        );
    }

    [Test]
    public void Read_NullToken_ReturnsDefault()
    {
        string json = "{\"filter\":null}";

        KvpHolder? result = PingenSerialisationHelper.Deserialize<KvpHolder>(json);

        result.ShouldNotBeNull();
        result!.Filter.Key.ShouldBeNull();
        result.Filter.Value.ShouldBeNull();
    }

    [Test]
    public void Read_NumericValueInsideJsonString_RoundTripsAsJsonElement()
    {
        string json = "{\"filter\":\"{\\\"count\\\":42}\"}";

        KvpHolder? result = PingenSerialisationHelper.Deserialize<KvpHolder>(json);

        result.ShouldNotBeNull();
        result!.Filter.Key.ShouldBe("count");
        JsonElement element = result.Filter.Value.ShouldBeOfType<JsonElement>();
        element.GetInt32().ShouldBe(42);
    }

    private sealed record KvpHolder
    {
        [JsonPropertyName("filter")]
        public KeyValuePair<string, object> Filter { get; init; }
    }

    private sealed record KvpListHolder
    {
        [JsonPropertyName("filters")]
        public IReadOnlyList<KeyValuePair<string, object>>? Filters { get; init; }
    }
}

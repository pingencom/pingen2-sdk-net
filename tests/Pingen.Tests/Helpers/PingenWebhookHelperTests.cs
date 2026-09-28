using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Tests.Helpers;

public class PingenWebhookHelperTests
{
    private static readonly string SamplePayload = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "webhook_sample.json"));

    private static readonly string DeliveredPayload = File.ReadAllText(
        Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "webhook_delivered_sample.json")
    );

    private static readonly string DeliverablesEventPayload = File.ReadAllText(
        Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "webhook_sent_deliverables_sample.json")
    );

    private static readonly string DeliverableOnlyPayload = File.ReadAllText(
        Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "webhook_undeliverable_deliverable_only_sample.json")
    );

    [Test]
    public async Task ValidateWebhook_ValidSignature_ReturnsTrue()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, SamplePayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, signature, stream);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task ValidateWebhook_InvalidSignature_ReturnsFalse()
    {
        const string signingKey = "test-signing-key";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, "invalid-signature", stream);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task ValidateWebhookAndGetData_InvalidSignature_Throws()
    {
        const string signingKey = "test-signing-key";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        await Should.ThrowAsync<Exception>(async () => await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, "invalid-sig", stream));
    }

    [Test]
    public async Task ValidateWebhookAndGetData_ValidSignature_ReturnsData()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, SamplePayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        (WebhookEventData? webhookEventData, Data<Organisation>? organisationData, Data<Letter>? letterData, Data<LetterEvent>? letterEventData) =
            await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldSatisfyAllConditions(
            () => webhookEventData.ShouldNotBeNull(),
            () => organisationData.ShouldNotBeNull(),
            () => letterData.ShouldNotBeNull(),
            () => letterEventData.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task ValidateWebhook_DifferentKey_ReturnsFalse()
    {
        const string signingKey = "correct-key";
        const string wrongKey = "wrong-key";
        string signature = ComputeHmacSha256(signingKey, SamplePayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(wrongKey, signature, stream);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task ValidateWebhook_EmptySigningKey_StillComputesHash()
    {
        const string signingKey = "";
        string signature = ComputeHmacSha256(signingKey, SamplePayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, signature, stream);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task ValidateWebhook_NonHexSignature_ReturnsFalse()
    {
        const string signingKey = "test-signing-key";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, "not-hex-zzz", stream);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task ValidateWebhook_OddLengthHexSignature_ReturnsFalse()
    {
        const string signingKey = "test-signing-key";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, "abc", stream);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task ValidateWebhook_UppercaseHexSignature_ReturnsTrue()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, SamplePayload).ToUpperInvariant();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SamplePayload));

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, signature, stream);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task ValidateWebhook_EmptyPayload_ComputesHashOverEmptyStream()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, string.Empty);
        using var stream = new MemoryStream();

        bool result = await PingenWebhookHelper.ValidateWebhook(signingKey, signature, stream);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task ValidateWebhookAndGetData_MalformedJson_ThrowsJsonException()
    {
        const string signingKey = "test-signing-key";
        const string malformed = "{not-valid-json";
        string signature = ComputeHmacSha256(signingKey, malformed);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(malformed));

        await Should.ThrowAsync<JsonException>(async () => await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream));
    }

    [TestCase("webhook_issues")]
    [TestCase("webhook_sent")]
    [TestCase("webhook_undeliverable")]
    [TestCase("webhook_delivered")]
    public async Task ValidateWebhookAndGetData_AllIncludedRelationshipsResolved_ForAllEventTypes(string apiDataType)
    {
        const string signingKey = "test-signing-key";
        string payload = BuildPayloadForType(apiDataType);
        string signature = ComputeHmacSha256(signingKey, payload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        (WebhookEventData? webhookEventData, Data<Organisation>? organisationData, Data<Letter>? letterData, Data<LetterEvent>? letterEventData) =
            await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldSatisfyAllConditions(
            () => webhookEventData.ShouldNotBeNull(),
            () => organisationData.ShouldNotBeNull(),
            () => letterData.ShouldNotBeNull(),
            () => letterEventData.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task ValidateWebhookAndGetData_WebhookSentType_DeserializesCorrectly()
    {
        const string signingKey = "test-signing-key";
        string payload = BuildPayloadForType("webhook_sent");
        string signature = ComputeHmacSha256(signingKey, payload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        (WebhookEventData? webhookEventData, _, _, _) = await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldNotBeNull();
        webhookEventData!.Type.ShouldBe("webhook_sent");
    }

    [Test]
    public async Task ValidateWebhookAndGetData_WebhookUndeliverableType_DeserializesCorrectly()
    {
        const string signingKey = "test-signing-key";
        string payload = BuildPayloadForType("webhook_undeliverable");
        string signature = ComputeHmacSha256(signingKey, payload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        (WebhookEventData? webhookEventData, _, _, _) = await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldNotBeNull();
        webhookEventData!.Type.ShouldBe("webhook_undeliverable");
    }

    [Test]
    public async Task ValidateWebhookAndGetData_WebhookDeliveredPayload_DeserializesWithoutLoss()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, DeliveredPayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(DeliveredPayload));

        (WebhookEventData? webhookEventData, Data<Organisation>? organisationData, Data<Letter>? letterData, Data<LetterEvent>? letterEventData) =
            await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldSatisfyAllConditions(
            () => webhookEventData.ShouldNotBeNull(),
            () => webhookEventData!.Type.ShouldBe("webhook_delivered"),
            () => webhookEventData!.Attributes.Reason.ShouldBeNull(),
            () => webhookEventData!.Attributes.Url.ShouldNotBeNull(),
            () => webhookEventData!.Attributes.CreatedAt.ShouldNotBeNull(),
            () => webhookEventData!.Relationships.Organisation.Data.Type.ShouldBe("organisations"),
            () => webhookEventData!.Relationships.Letter.ShouldNotBeNull(),
            () => webhookEventData!.Relationships.Letter!.Data.Type.ShouldBe("letters"),
            () => webhookEventData!.Relationships.Deliverable.ShouldBeNull(),
            () => webhookEventData!.Relationships.Event.Data.Type.ShouldBe("letters_events"),
            () => organisationData.ShouldNotBeNull(),
            () => letterData.ShouldNotBeNull(),
            () => letterEventData.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task ValidateWebhookAndGetData_DeliverablesEventRelationship_DeserializesAndResolvesSingleEvent()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, DeliverablesEventPayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(DeliverablesEventPayload));

        (WebhookEventData? webhookEventData, Data<Organisation>? organisationData, Data<Letter>? letterData, Data<LetterEvent>? letterEventData) =
            await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldSatisfyAllConditions(
            () => webhookEventData.ShouldNotBeNull(),
            () => webhookEventData!.Type.ShouldBe("webhook_sent"),
            () => webhookEventData!.Relationships.Organisation.Data.Type.ShouldBe("organisations"),
            () => webhookEventData!.Relationships.Letter.ShouldNotBeNull(),
            () => webhookEventData!.Relationships.Letter!.Data.Type.ShouldBe("letters"),
            () => webhookEventData!.Relationships.Deliverable.ShouldNotBeNull(),
            () => webhookEventData!.Relationships.Deliverable!.Data.Type.ShouldBe("letters"),
            () => webhookEventData!.Relationships.Deliverable!.Data.Id.ShouldBe(webhookEventData!.Relationships.Letter!.Data.Id),
            () => webhookEventData!.Relationships.Event.Data.Type.ShouldBe("deliverables_events"),
            () => organisationData.ShouldNotBeNull(),
            () => letterData.ShouldNotBeNull(),
            () => letterEventData.ShouldNotBeNull(),
            () => letterEventData!.Attributes.Code.ShouldBe("transferred_to_distributor")
        );
    }

    [Test]
    public async Task ValidateWebhookAndGetData_LegacyLetterShapeDropped_StillResolvesEverything()
    {
        const string signingKey = "test-signing-key";
        string signature = ComputeHmacSha256(signingKey, DeliverableOnlyPayload);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(DeliverableOnlyPayload));

        (WebhookEventData? webhookEventData, Data<Organisation>? organisationData, Data<Letter>? letterData, Data<LetterEvent>? letterEventData) =
            await PingenWebhookHelper.ValidateWebhookAndGetData(signingKey, signature, stream);

        webhookEventData.ShouldSatisfyAllConditions(
            () => webhookEventData.ShouldNotBeNull(),
            () => webhookEventData!.Type.ShouldBe("webhook_undeliverable"),
            () => webhookEventData!.Relationships.Letter.ShouldBeNull(),
            () => webhookEventData!.Relationships.Deliverable.ShouldNotBeNull(),
            () => webhookEventData!.Relationships.Deliverable!.Data.Type.ShouldBe("letters"),
            () => webhookEventData!.Relationships.Event.Data.Type.ShouldBe("deliverables_events"),
            () => organisationData.ShouldNotBeNull(),
            () => letterData.ShouldNotBeNull(),
            () => letterEventData.ShouldNotBeNull(),
            () => letterEventData!.Attributes.Code.ShouldBe("undeliverable"),
            () => webhookEventData!.Attributes.CorrectedAddress.ShouldNotBeNull(),
            () => webhookEventData!.Attributes.CorrectedAddress!.Zip.ShouldBe("8051"),
            () => webhookEventData!.Attributes.CorrectedAddress!.Number.ShouldBe("50A")
        );
    }

    private static string ComputeHmacSha256(string key, string data)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA256(keyBytes);
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string BuildPayloadForType(string apiDataType) => SamplePayload.Replace("\"type\": \"webhook_issues\"", $"\"type\": \"{apiDataType}\"");
}

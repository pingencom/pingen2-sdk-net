using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Helpers;

namespace Pingen.Tests.Enums;

public class AllEnumsSerializationTests
{
    // -------------------------------------------------------------------
    // Section A: BatchGroupingType (real enum, 2 values)
    // -------------------------------------------------------------------

    [TestCase(BatchGroupingType.zip, "\"zip\"")]
    [TestCase(BatchGroupingType.merge, "\"merge\"")]
    public void BatchGroupingType_Serializes(BatchGroupingType value, string expectedJson) => PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"zip\"", BatchGroupingType.zip)]
    [TestCase("\"merge\"", BatchGroupingType.merge)]
    public void BatchGroupingType_Deserializes(string json, BatchGroupingType expected) =>
        PingenSerialisationHelper.Deserialize<BatchGroupingType>(json).ShouldBe(expected);

    [Test]
    public void BatchGroupingType_AllValuesAreCovered() => Enum.GetValues<BatchGroupingType>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section B: BatchGroupingOptionsSplitType (real enum, 4 values)
    // -------------------------------------------------------------------

    [TestCase(BatchGroupingOptionsSplitType.file, "\"file\"")]
    [TestCase(BatchGroupingOptionsSplitType.page, "\"page\"")]
    [TestCase(BatchGroupingOptionsSplitType.custom, "\"custom\"")]
    [TestCase(BatchGroupingOptionsSplitType.qr_invoice, "\"qr_invoice\"")]
    public void BatchGroupingOptionsSplitType_Serializes(BatchGroupingOptionsSplitType value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"file\"", BatchGroupingOptionsSplitType.file)]
    [TestCase("\"page\"", BatchGroupingOptionsSplitType.page)]
    [TestCase("\"custom\"", BatchGroupingOptionsSplitType.custom)]
    [TestCase("\"qr_invoice\"", BatchGroupingOptionsSplitType.qr_invoice)]
    public void BatchGroupingOptionsSplitType_Deserializes(string json, BatchGroupingOptionsSplitType expected) =>
        PingenSerialisationHelper.Deserialize<BatchGroupingOptionsSplitType>(json).ShouldBe(expected);

    [Test]
    public void BatchGroupingOptionsSplitType_AllValuesAreCovered() => Enum.GetValues<BatchGroupingOptionsSplitType>().Length.ShouldBe(4);

    // -------------------------------------------------------------------
    // Section C: BatchGroupingOptionsSplitPosition (real enum, 2 values)
    // -------------------------------------------------------------------

    [TestCase(BatchGroupingOptionsSplitPosition.first_page, "\"first_page\"")]
    [TestCase(BatchGroupingOptionsSplitPosition.last_page, "\"last_page\"")]
    public void BatchGroupingOptionsSplitPosition_Serializes(BatchGroupingOptionsSplitPosition value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"first_page\"", BatchGroupingOptionsSplitPosition.first_page)]
    [TestCase("\"last_page\"", BatchGroupingOptionsSplitPosition.last_page)]
    public void BatchGroupingOptionsSplitPosition_Deserializes(string json, BatchGroupingOptionsSplitPosition expected) =>
        PingenSerialisationHelper.Deserialize<BatchGroupingOptionsSplitPosition>(json).ShouldBe(expected);

    [Test]
    public void BatchGroupingOptionsSplitPosition_AllValuesAreCovered() => Enum.GetValues<BatchGroupingOptionsSplitPosition>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section F: CollectionSortDirection (real enum, 2 values, uppercase identifiers)
    // -------------------------------------------------------------------

    [TestCase(CollectionSortDirection.ASC, "\"ASC\"")]
    [TestCase(CollectionSortDirection.DESC, "\"DESC\"")]
    public void CollectionSortDirection_Serializes(CollectionSortDirection value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"ASC\"", CollectionSortDirection.ASC)]
    [TestCase("\"DESC\"", CollectionSortDirection.DESC)]
    public void CollectionSortDirection_Deserializes(string json, CollectionSortDirection expected) =>
        PingenSerialisationHelper.Deserialize<CollectionSortDirection>(json).ShouldBe(expected);

    [Test]
    public void CollectionSortDirection_AllValuesAreCovered() => Enum.GetValues<CollectionSortDirection>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section G: WebhookEventCategory (static class of const string, 5 constants)
    // -------------------------------------------------------------------

    [TestCase("issues", WebhookEventCategory.Issues)]
    [TestCase("undeliverable", WebhookEventCategory.Undeliverable)]
    [TestCase("sent", WebhookEventCategory.Sent)]
    [TestCase("delivered", WebhookEventCategory.Delivered)]
    [TestCase("channel_subscriptions", WebhookEventCategory.ChannelSubscriptions)]
    public void WebhookEventCategory_HasExpectedConstantValue(string expected, string actual) => actual.ShouldBe(expected);

    // -------------------------------------------------------------------
    // Section H: PingenApiAbility (real enum, 3 values)
    // -------------------------------------------------------------------

    [TestCase(PingenApiAbility.ok, "\"ok\"")]
    [TestCase(PingenApiAbility.state, "\"state\"")]
    [TestCase(PingenApiAbility.permission, "\"permission\"")]
    public void PingenApiAbility_Serializes(PingenApiAbility value, string expectedJson) => PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"ok\"", PingenApiAbility.ok)]
    [TestCase("\"state\"", PingenApiAbility.state)]
    [TestCase("\"permission\"", PingenApiAbility.permission)]
    public void PingenApiAbility_Deserializes(string json, PingenApiAbility expected) =>
        PingenSerialisationHelper.Deserialize<PingenApiAbility>(json).ShouldBe(expected);

    [Test]
    public void PingenApiAbility_AllValuesAreCovered() => Enum.GetValues<PingenApiAbility>().Length.ShouldBe(3);

    // -------------------------------------------------------------------
    // Section K: CollectionFilterOperator (static class of const string, 2 constants)
    // -------------------------------------------------------------------

    [TestCase("and", CollectionFilterOperator.And)]
    [TestCase("or", CollectionFilterOperator.Or)]
    public void CollectionFilterOperator_HasExpectedConstantValue(string expected, string actual) => actual.ShouldBe(expected);

    [TestCase(CollectionFilterOperator.And, "\"and\"")]
    [TestCase(CollectionFilterOperator.Or, "\"or\"")]
    public void CollectionFilterOperator_SerializesAsJsonString(string value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    // -------------------------------------------------------------------
    // Section L: PingenApiLanguage (static class of const string, 6 constants)
    // -------------------------------------------------------------------

    [TestCase("en-GB", PingenApiLanguage.EnGB)]
    [TestCase("de-DE", PingenApiLanguage.DeDE)]
    [TestCase("de-CH", PingenApiLanguage.DeCH)]
    [TestCase("nl-NL", PingenApiLanguage.NlNL)]
    [TestCase("fr-FR", PingenApiLanguage.FrFR)]
    [TestCase("es-ES", PingenApiLanguage.EsES)]
    public void PingenApiLanguage_HasExpectedConstantValue(string expected, string actual) => actual.ShouldBe(expected);

    [TestCase(PingenApiLanguage.EnGB, "\"en-GB\"")]
    [TestCase(PingenApiLanguage.DeDE, "\"de-DE\"")]
    [TestCase(PingenApiLanguage.DeCH, "\"de-CH\"")]
    [TestCase(PingenApiLanguage.NlNL, "\"nl-NL\"")]
    [TestCase(PingenApiLanguage.FrFR, "\"fr-FR\"")]
    [TestCase(PingenApiLanguage.EsES, "\"es-ES\"")]
    public void PingenApiLanguage_SerializesAsJsonString(string value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);
}

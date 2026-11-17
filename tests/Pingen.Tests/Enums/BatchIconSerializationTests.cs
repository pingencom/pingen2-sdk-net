using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Helpers;

namespace Pingen.Tests.Enums;

public class BatchIconSerializationTests
{
    [TestCase(BatchIcon.campaign, "\"campaign\"")]
    [TestCase(BatchIcon.megaphone, "\"megaphone\"")]
    [TestCase(BatchIcon.waveHand, "\"wave-hand\"")]
    [TestCase(BatchIcon.flash, "\"flash\"")]
    [TestCase(BatchIcon.rocket, "\"rocket\"")]
    [TestCase(BatchIcon.bell, "\"bell\"")]
    [TestCase(BatchIcon.percentTag, "\"percent-tag\"")]
    [TestCase(BatchIcon.percentBadge, "\"percent-badge\"")]
    [TestCase(BatchIcon.present, "\"present\"")]
    [TestCase(BatchIcon.receipt, "\"receipt\"")]
    [TestCase(BatchIcon.document, "\"document\"")]
    [TestCase(BatchIcon.information, "\"information\"")]
    [TestCase(BatchIcon.calendar, "\"calendar\"")]
    [TestCase(BatchIcon.newspaper, "\"newspaper\"")]
    [TestCase(BatchIcon.crown, "\"crown\"")]
    [TestCase(BatchIcon.virus, "\"virus\"")]
    public void BatchIcon_Serializes(BatchIcon value, string expectedJson) => PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"campaign\"", BatchIcon.campaign)]
    [TestCase("\"megaphone\"", BatchIcon.megaphone)]
    [TestCase("\"wave-hand\"", BatchIcon.waveHand)]
    [TestCase("\"flash\"", BatchIcon.flash)]
    [TestCase("\"rocket\"", BatchIcon.rocket)]
    [TestCase("\"bell\"", BatchIcon.bell)]
    [TestCase("\"percent-tag\"", BatchIcon.percentTag)]
    [TestCase("\"percent-badge\"", BatchIcon.percentBadge)]
    [TestCase("\"present\"", BatchIcon.present)]
    [TestCase("\"receipt\"", BatchIcon.receipt)]
    [TestCase("\"document\"", BatchIcon.document)]
    [TestCase("\"information\"", BatchIcon.information)]
    [TestCase("\"calendar\"", BatchIcon.calendar)]
    [TestCase("\"newspaper\"", BatchIcon.newspaper)]
    [TestCase("\"crown\"", BatchIcon.crown)]
    [TestCase("\"virus\"", BatchIcon.virus)]
    public void BatchIcon_Deserializes(string json, BatchIcon expected) => PingenSerialisationHelper.Deserialize<BatchIcon>(json).ShouldBe(expected);

    [TestCase(BatchIcon.waveHand)]
    [TestCase(BatchIcon.percentTag)]
    [TestCase(BatchIcon.percentBadge)]
    public void BatchIcon_HyphenatedValues_RoundTrip(BatchIcon value)
    {
        string json = PingenSerialisationHelper.Serialize(value);

        PingenSerialisationHelper.Deserialize<BatchIcon>(json).ShouldBe(value);
    }

    [Test]
    public void BatchIcon_AllValuesAreCovered() => Enum.GetValues<BatchIcon>().Length.ShouldBe(16);
}

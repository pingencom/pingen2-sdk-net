using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Helpers;

namespace Pingen.Tests.Enums;

public class LetterEnumSerializationTests
{
    // -------------------------------------------------------------------
    // Section A: LetterPrintMode (real enum, 2 values)
    // -------------------------------------------------------------------

    [TestCase(LetterPrintMode.simplex, "\"simplex\"")]
    [TestCase(LetterPrintMode.duplex, "\"duplex\"")]
    public void LetterPrintMode_Serializes(LetterPrintMode value, string expectedJson) => PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"simplex\"", LetterPrintMode.simplex)]
    [TestCase("\"duplex\"", LetterPrintMode.duplex)]
    public void LetterPrintMode_Deserializes(string json, LetterPrintMode expected) =>
        PingenSerialisationHelper.Deserialize<LetterPrintMode>(json).ShouldBe(expected);

    [Test]
    public void LetterPrintMode_AllValuesAreCovered() => Enum.GetValues<LetterPrintMode>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section B: LetterAddressPosition (real enum, 2 values)
    // -------------------------------------------------------------------

    [TestCase(LetterAddressPosition.left, "\"left\"")]
    [TestCase(LetterAddressPosition.right, "\"right\"")]
    public void LetterAddressPosition_Serializes(LetterAddressPosition value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"left\"", LetterAddressPosition.left)]
    [TestCase("\"right\"", LetterAddressPosition.right)]
    public void LetterAddressPosition_Deserializes(string json, LetterAddressPosition expected) =>
        PingenSerialisationHelper.Deserialize<LetterAddressPosition>(json).ShouldBe(expected);

    [Test]
    public void LetterAddressPosition_AllValuesAreCovered() => Enum.GetValues<LetterAddressPosition>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section C: LetterPrintSpectrum (real enum, 2 values)
    // -------------------------------------------------------------------

    [TestCase(LetterPrintSpectrum.grayscale, "\"grayscale\"")]
    [TestCase(LetterPrintSpectrum.color, "\"color\"")]
    public void LetterPrintSpectrum_Serializes(LetterPrintSpectrum value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);

    [TestCase("\"grayscale\"", LetterPrintSpectrum.grayscale)]
    [TestCase("\"color\"", LetterPrintSpectrum.color)]
    public void LetterPrintSpectrum_Deserializes(string json, LetterPrintSpectrum expected) =>
        PingenSerialisationHelper.Deserialize<LetterPrintSpectrum>(json).ShouldBe(expected);

    [Test]
    public void LetterPrintSpectrum_AllValuesAreCovered() => Enum.GetValues<LetterPrintSpectrum>().Length.ShouldBe(2);

    // -------------------------------------------------------------------
    // Section E: LetterCreateDeliveryProduct (static class of const string, 5 constants)
    // -------------------------------------------------------------------

    [TestCase("fast", LetterCreateDeliveryProduct.Fast)]
    [TestCase("cheap", LetterCreateDeliveryProduct.Cheap)]
    [TestCase("bulk", LetterCreateDeliveryProduct.Bulk)]
    [TestCase("premium", LetterCreateDeliveryProduct.Premium)]
    [TestCase("registered", LetterCreateDeliveryProduct.Registered)]
    public void LetterCreateDeliveryProduct_HasExpectedConstantValue(string expected, string actual) => actual.ShouldBe(expected);

    [TestCase(LetterCreateDeliveryProduct.Fast, "\"fast\"")]
    [TestCase(LetterCreateDeliveryProduct.Cheap, "\"cheap\"")]
    [TestCase(LetterCreateDeliveryProduct.Bulk, "\"bulk\"")]
    [TestCase(LetterCreateDeliveryProduct.Premium, "\"premium\"")]
    [TestCase(LetterCreateDeliveryProduct.Registered, "\"registered\"")]
    public void LetterCreateDeliveryProduct_SerializesAsJsonString(string value, string expectedJson) =>
        PingenSerialisationHelper.Serialize(value).ShouldBe(expectedJson);
}

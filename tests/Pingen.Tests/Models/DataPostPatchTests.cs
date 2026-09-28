using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Letters.Views;

namespace Pingen.Tests.Models;

public class DataPostPatchTests
{
    [Test]
    public void DataPost_StoresTypeAndAttributes()
    {
        LetterCreate attributes = CreateLetterCreateAttributes();

        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = attributes };

        dataPost.ShouldSatisfyAllConditions(() => dataPost.Type.ShouldBe("letters"), () => dataPost.Attributes.ShouldBeSameAs(attributes));
    }

    [Test]
    public void DataPost_SerializesToExpectedJson()
    {
        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = CreateLetterCreateAttributes() };

        string json = PingenSerialisationHelper.Serialize(new { data = dataPost });

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"type\":\"letters\""),
            () => json.ShouldContain("\"attributes\""),
            () => json.ShouldContain("\"file_original_name\":\"test.pdf\"")
        );
    }

    [Test]
    public void DataPatch_StoresIdTypeAndAttributes()
    {
        var attributes = new LetterSend
        {
            DeliveryProduct = "cheap",
            PrintMode = LetterPrintMode.simplex,
            PrintSpectrum = LetterPrintSpectrum.color,
        };

        var dataPatch = new DataPatch<LetterSend>
        {
            Id = "letter-123",
            Type = "letters",
            Attributes = attributes,
        };

        dataPatch.ShouldSatisfyAllConditions(
            () => dataPatch.Id.ShouldBe("letter-123"),
            () => dataPatch.Type.ShouldBe("letters"),
            () => dataPatch.Attributes.ShouldBeSameAs(attributes)
        );
    }

    [Test]
    public void DataPatch_SerializesToJsonWithId()
    {
        var dataPatch = new DataPatch<LetterSend>
        {
            Id = "letter-456",
            Type = "letters",
            Attributes = new LetterSend
            {
                DeliveryProduct = "cheap",
                PrintMode = LetterPrintMode.simplex,
                PrintSpectrum = LetterPrintSpectrum.color,
            },
        };

        string json = PingenSerialisationHelper.Serialize(new { data = dataPatch });

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"id\":\"letter-456\""),
            () => json.ShouldContain("\"type\":\"letters\""),
            () => json.ShouldContain("\"attributes\"")
        );
    }

    [Test]
    public void DataPost_SerializesType_AsSnakeCaseEnumValue()
    {
        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = CreateLetterCreateAttributes() };

        string json = PingenSerialisationHelper.Serialize(dataPost);

        json.ShouldContain("\"type\":\"letters\"");
    }

    [Test]
    public void DataPost_OmitsRelationships_WhenUsingNonRelationshipsGenericArity()
    {
        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = CreateLetterCreateAttributes() };

        string json = PingenSerialisationHelper.Serialize(dataPost);

        json.ShouldNotContain("relationships");
    }

    [Test]
    public void DataPostWithRelationships_SerializesRelationshipsBlock()
    {
        var dataPost = new DataPost<LetterCreate, LetterCreateRelationships>
        {
            Type = "letters",
            Attributes = CreateLetterCreateAttributes(),
            Relationships = LetterCreateRelationships.Create("preset-1"),
        };

        string json = PingenSerialisationHelper.Serialize(dataPost);

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"relationships\""),
            () => json.ShouldContain("\"preset\""),
            () => json.ShouldContain("\"id\":\"preset-1\""),
            () => json.ShouldContain("\"type\":\"presets\"")
        );
    }

    [Test]
    public void DataPostWithRelationships_NullRelationships_OmittedFromJson()
    {
        var dataPost = new DataPost<LetterCreate, LetterCreateRelationships>
        {
            Type = "letters",
            Attributes = CreateLetterCreateAttributes(),
            Relationships = null,
        };

        string json = PingenSerialisationHelper.Serialize(dataPost);

        json.ShouldNotContain("\"relationships\"");
    }

    [Test]
    public void DataPost_NullOptionalAttribute_OmittedFromJson()
    {
        LetterCreate attributes = CreateLetterCreateAttributes() with { MetaData = null };

        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = attributes };

        string json = PingenSerialisationHelper.Serialize(dataPost);

        json.ShouldNotContain("\"meta_data\"");
    }

    [Test]
    public void DataPatch_InheritsFromDataPost_TypeAndAttributes()
    {
        var dataPatch = new DataPatch<LetterSend>
        {
            Id = "letter-789",
            Type = "letters",
            Attributes = new LetterSend
            {
                DeliveryProduct = "cheap",
                PrintMode = LetterPrintMode.simplex,
                PrintSpectrum = LetterPrintSpectrum.color,
            },
        };

        dataPatch.ShouldBeAssignableTo<DataPost<LetterSend>>();
    }

    [Test]
    public void DataPatch_SerializesIdTypeAttributes_NotRelationships()
    {
        var dataPatch = new DataPatch<LetterSend>
        {
            Id = "letter-x",
            Type = "letters",
            Attributes = new LetterSend
            {
                DeliveryProduct = "cheap",
                PrintMode = LetterPrintMode.simplex,
                PrintSpectrum = LetterPrintSpectrum.color,
            },
        };

        string json = PingenSerialisationHelper.Serialize(dataPatch);

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"id\":\"letter-x\""),
            () => json.ShouldContain("\"type\":\"letters\""),
            () => json.ShouldContain("\"attributes\""),
            () => json.ShouldNotContain("\"relationships\"")
        );
    }

    [Test]
    public void DataPost_SerializeThenWrapInDataEnvelope_ProducesJsonApiShape()
    {
        var dataPost = new DataPost<LetterCreate> { Type = "letters", Attributes = CreateLetterCreateAttributes() };

        string json = PingenSerialisationHelper.Serialize(new { data = dataPost });

        json.ShouldSatisfyAllConditions(
            () => json.ShouldStartWith("{\"data\":{"),
            () => json.ShouldContain("\"type\":\"letters\""),
            () => json.ShouldContain("\"attributes\""),
            () => json.ShouldContain("\"file_original_name\":\"test.pdf\"")
        );
    }

    [Test]
    public void DataPost_ReferenceEquality_AttributesShareInstance()
    {
        LetterCreate attributes = CreateLetterCreateAttributes();

        var p1 = new DataPost<LetterCreate> { Type = "letters", Attributes = attributes };

        var p2 = new DataPost<LetterCreate> { Type = "letters", Attributes = attributes };

        PingenSerialisationHelper.Serialize(p1).ShouldBe(PingenSerialisationHelper.Serialize(p2));
    }

    private static LetterCreate CreateLetterCreateAttributes() =>
        new()
        {
            FileOriginalName = "test.pdf",
            FileUrl = "https://example.com/test.pdf",
            FileUrlSignature = "sig123",
            AddressPosition = LetterAddressPosition.left,
            AutoSend = false,
            DeliveryProduct = "cheap",
            PrintMode = LetterPrintMode.simplex,
            PrintSpectrum = LetterPrintSpectrum.grayscale,
        };
}

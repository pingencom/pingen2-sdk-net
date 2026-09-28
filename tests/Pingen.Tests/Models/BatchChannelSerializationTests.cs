using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Batches.Views;

namespace Pingen.Tests.Models;

public class BatchChannelSerializationTests
{
    private static BatchCreate PostalCreate() =>
        new()
        {
            Name = "Invoicing May",
            Icon = BatchIcon.receipt,
            FileOriginalName = "invoices.pdf",
            FileUrl = "https://example.com/invoices.pdf",
            FileUrlSignature = "sig123",
            GroupingType = BatchGroupingType.zip,
            GroupingOptionsSplitType = BatchGroupingOptionsSplitType.file,
        };

    [Test]
    public void BatchCreate_SerializesChannelType_WhenSet()
    {
        BatchCreate attributes = PostalCreate() with { ChannelType = BatchChannelTypes.Email };

        string json = PingenSerialisationHelper.Serialize(attributes);

        json.ShouldContain("\"channel_type\":\"email\"");
    }

    [Test]
    public void BatchCreate_OmitsChannelType_WhenNull()
    {
        string json = PingenSerialisationHelper.Serialize(PostalCreate());

        json.ShouldNotContain("channel_type");
    }

    [Test]
    public void BatchCreate_OmitsAddressPosition_WhenNull()
    {
        BatchCreate attributes = PostalCreate() with { ChannelType = BatchChannelTypes.Ebill };

        string json = PingenSerialisationHelper.Serialize(attributes);

        json.ShouldNotContain("address_position");
    }

    [Test]
    public void BatchCreate_SerializesAddressPosition_WhenSet()
    {
        BatchCreate attributes = PostalCreate() with { AddressPosition = LetterAddressPosition.left };

        string json = PingenSerialisationHelper.Serialize(attributes);

        json.ShouldContain("\"address_position\":\"left\"");
    }

    [Test]
    public void BatchSend_EmailChannel_SerializesDeliveryProductOnly()
    {
        var send = new BatchSend { DeliveryProduct = "electronic_email" };

        string json = PingenSerialisationHelper.Serialize(send);

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"delivery_product\":\"electronic_email\""),
            () => json.ShouldNotContain("print_mode"),
            () => json.ShouldNotContain("print_spectrum")
        );
    }

    [Test]
    public void BatchSend_EbillChannel_SerializesElectronicEbillProduct()
    {
        var send = new BatchSend { DeliveryProduct = "electronic_ebill" };

        string json = PingenSerialisationHelper.Serialize(send);

        json.ShouldContain("\"delivery_product\":\"electronic_ebill\"");
    }

    [Test]
    public void BatchSend_PostalChannel_SerializesPrintOptions()
    {
        var send = new BatchSend
        {
            DeliveryProduct = "fast",
            PrintMode = LetterPrintMode.simplex,
            PrintSpectrum = LetterPrintSpectrum.color,
        };

        string json = PingenSerialisationHelper.Serialize(send);

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"delivery_product\":\"fast\""),
            () => json.ShouldContain("\"print_mode\":\"simplex\""),
            () => json.ShouldContain("\"print_spectrum\":\"color\"")
        );
    }

    [Test]
    public void BatchSendRequest_Email_ProducesChannelTypeAndProduct()
    {
        string json = PingenSerialisationHelper.Serialize(new { data = BatchSendRequest.Email("batch-1") });

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"id\":\"batch-1\""),
            () => json.ShouldContain("\"type\":\"batches_channel_email_send\""),
            () => json.ShouldContain("\"delivery_product\":\"electronic_email\""),
            () => json.ShouldNotContain("print_mode")
        );
    }

    [Test]
    public void BatchSendRequest_Ebill_ProducesChannelTypeAndProduct()
    {
        string json = PingenSerialisationHelper.Serialize(new { data = BatchSendRequest.Ebill("batch-2") });

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"type\":\"batches_channel_ebill_send\""),
            () => json.ShouldContain("\"delivery_product\":\"electronic_ebill\"")
        );
    }

    [Test]
    public void BatchSendRequest_Post_ProducesChannelTypeAndPrintOptions()
    {
        string json = PingenSerialisationHelper.Serialize(
            new { data = BatchSendRequest.Post("batch-3", "fast", LetterPrintMode.simplex, LetterPrintSpectrum.color) }
        );

        json.ShouldSatisfyAllConditions(
            () => json.ShouldContain("\"type\":\"batches_channel_post_send\""),
            () => json.ShouldContain("\"delivery_product\":\"fast\""),
            () => json.ShouldContain("\"print_mode\":\"simplex\""),
            () => json.ShouldContain("\"print_spectrum\":\"color\"")
        );
    }
}

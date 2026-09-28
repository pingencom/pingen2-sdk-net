using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Batches.Views;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Ebills.Embedded;
using Pingen.Abstractions.Models.Ebills.Views;
using Pingen.Abstractions.Models.Emails;
using Pingen.Abstractions.Models.Emails.Embedded;
using Pingen.Abstractions.Models.Emails.Views;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Letters.Views;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class UploadAndCreateServiceTests : EndpointTestBase
{
    private void StubFileUpload(string s3Path)
    {
        string fileUploadResponse = JsonApiStubHelper.SingleResponse(
            "file-1",
            "file_uploads",
            new
            {
                url = $"{Server.Url}{s3Path}",
                url_signature = "upload-signature",
                expires_at = DateTime.UtcNow.AddHours(1).ToString("o"),
            }
        );

        Server.StubJsonGet("/file-upload", fileUploadResponse);
        Server.Given(Request.Create().WithPath(s3Path).UsingPut()).RespondWith(Response.Create().WithStatusCode(200));
    }

    [Test]
    public async Task Letters_UploadAndCreate_UploadsThenCreates()
    {
        StubFileUpload("/s3-upload-letter");
        Server.StubJsonPost(OrgPath("deliveries/letters"), PingenResponseFactory.SingleLetter("letter-uc"));

        using var stream = new MemoryStream([1, 2, 3]);
        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.UploadAndCreate(
            stream,
            new LetterUpload
            {
                FileOriginalName = "doc.pdf",
                AddressPosition = LetterAddressPosition.left,
                AutoSend = false,
                DeliveryProduct = "cheap",
                PrintMode = LetterPrintMode.simplex,
                PrintSpectrum = LetterPrintSpectrum.color,
            }
        );

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("letter-uc"));
    }

    [Test]
    public async Task Emails_UploadAndCreate_UploadsThenCreates()
    {
        StubFileUpload("/s3-upload-email");
        Server.StubJsonPost(OrgPath("deliveries/emails"), PingenResponseFactory.SingleEmail("email-uc"));

        using var stream = new MemoryStream([4, 5, 6]);
        ApiResult<SingleResult<EmailDataDetailed>> result = await Client.Emails.UploadAndCreate(
            stream,
            new EmailUpload
            {
                FileOriginalName = "invoice.pdf",
                AutoSend = false,
                MetaData = new EmailMetaData
                {
                    SenderName = "Sender",
                    RecipientEmail = "r@example.com",
                    RecipientName = "R",
                    ReplyEmail = "reply@example.com",
                    ReplyName = "Reply",
                    Subject = "Subject",
                    Content = "Content",
                },
            }
        );

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("email-uc"));
    }

    [Test]
    public async Task Batches_UploadAndCreate_UploadsThenCreates()
    {
        StubFileUpload("/s3-upload-batch");
        Server.StubJsonPost(OrgPath("batches"), PingenResponseFactory.SingleBatch("batch-uc"));

        using var stream = new MemoryStream([7, 8, 9]);
        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.UploadAndCreate(
            stream,
            new BatchUpload
            {
                Name = "Q1 mailing",
                Icon = BatchIcon.rocket,
                FileOriginalName = "mailing.pdf",
                AddressPosition = LetterAddressPosition.left,
                GroupingType = BatchGroupingType.merge,
                GroupingOptionsSplitType = BatchGroupingOptionsSplitType.file,
            }
        );

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("batch-uc"));
    }

    [Test]
    public async Task Batches_UploadAndCreate_EmailChannel_UploadsThenCreates()
    {
        StubFileUpload("/s3-upload-batch-email");
        Server.StubJsonPost(OrgPath("batches"), PingenResponseFactory.SingleBatch("batch-email-uc"));

        using var stream = new MemoryStream([4, 5, 6]);
        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.UploadAndCreate(
            stream,
            new BatchUpload
            {
                Name = "Q1 email batch",
                ChannelType = BatchChannelTypes.Email,
                Icon = BatchIcon.rocket,
                FileOriginalName = "mailing.pdf",
                GroupingType = BatchGroupingType.merge,
                GroupingOptionsSplitType = BatchGroupingOptionsSplitType.file,
            }
        );

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("batch-email-uc"));
    }

    [Test]
    public async Task Ebills_UploadAndCreate_UploadsThenCreates()
    {
        StubFileUpload("/s3-upload-ebill");
        Server.StubJsonPost(OrgPath("deliveries/ebills"), PingenResponseFactory.SingleEbill("ebill-uc"));

        using var stream = new MemoryStream([1, 1, 1]);
        ApiResult<SingleResult<EbillDataDetailed>> result = await Client.Ebills.UploadAndCreate(
            stream,
            new EbillUpload
            {
                FileOriginalName = "invoice.pdf",
                AutoSend = false,
                MetaData = new EbillMetaData
                {
                    InvoiceNumber = "INV-0001",
                    InvoiceDate = "2026-01-01",
                    InvoiceDueDate = "2026-01-31",
                    RecipientIdentifier = "41010560000000000",
                },
            }
        );

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("ebill-uc"));
    }

    private static LetterUpload SampleLetterUpload() =>
        new()
        {
            FileOriginalName = "doc.pdf",
            AddressPosition = LetterAddressPosition.left,
            AutoSend = false,
            DeliveryProduct = "cheap",
            PrintMode = LetterPrintMode.simplex,
            PrintSpectrum = LetterPrintSpectrum.color,
        };

    [Test]
    public async Task UploadAndCreate_WhenFileUploadUrlRequestFails_ThrowsPingenApiErrorException()
    {
        Server.StubError("/file-upload", "GET", PingenResponseFactory.ErrorResponse("Server Error", "Cannot allocate upload slot", "500"), 500);

        using var stream = new MemoryStream([1, 2, 3]);

        await Should.ThrowAsync<PingenApiErrorException>(async () => await Client.Letters.UploadAndCreate(stream, SampleLetterUpload()));
    }

    [Test]
    public async Task UploadAndCreate_WhenStorageUploadFails_ThrowsInvalidOperationException()
    {
        var fileUploadResponse = JsonApiStubHelper.SingleResponse(
            "file-1",
            "file_uploads",
            new
            {
                url = $"{Server.Url}/s3-fail",
                url_signature = "upload-signature",
                expires_at = DateTime.UtcNow.AddHours(1).ToString("o"),
            }
        );
        Server.StubJsonGet("/file-upload", fileUploadResponse);
        Server.Given(Request.Create().WithPath("/s3-fail").UsingPut()).RespondWith(Response.Create().WithStatusCode(403));

        using var stream = new MemoryStream([1, 2, 3]);

        await Should.ThrowAsync<InvalidOperationException>(async () => await Client.Letters.UploadAndCreate(stream, SampleLetterUpload()));
    }
}

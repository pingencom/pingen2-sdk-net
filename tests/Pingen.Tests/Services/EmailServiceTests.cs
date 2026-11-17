using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.EmailEvents;
using Pingen.Abstractions.Models.Emails;
using Pingen.Abstractions.Models.Emails.Embedded;
using Pingen.Abstractions.Models.Emails.Views;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class EmailServiceTests : EndpointTestBase
{
    private const string EmailsPath = "deliveries/emails";

    private static DataPost<EmailCreate, EmailCreateRelationships> CreatePayload() =>
        new()
        {
            Type = "emails",
            Attributes = new EmailCreate
            {
                FileOriginalName = "invoice.pdf",
                FileUrl = "https://s3.example.com/upload/abc",
                FileUrlSignature = "signature-value",
                AutoSend = true,
                MetaData = new EmailMetaData
                {
                    SenderName = "Sender",
                    RecipientEmail = "recipient@example.com",
                    RecipientName = "Recipient",
                    ReplyEmail = "reply@example.com",
                    ReplyName = "Reply",
                    Subject = "Your invoice",
                    Content = "Please find your invoice attached.",
                },
            },
            Relationships = EmailCreateRelationships.Create("preset-1"),
        };

    [Test]
    public async Task GetPage_ShouldReturnEmails()
    {
        Server.StubJsonGet(OrgPath(EmailsPath), PingenResponseFactory.EmailCollection());

        ApiResult<CollectionResult<EmailData>> result = await Client.Emails.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.RecipientIdentifier.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Get_ShouldReturnEmailDetail()
    {
        Server.StubJsonGet(OrgPath($"{EmailsPath}/email-1"), PingenResponseFactory.SingleEmail("email-1"));

        ApiResult<SingleResult<EmailDataDetailed>> result = await Client.Emails.Get("email-1");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe("email-1"),
            () => result.Data!.Data.Meta.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task Create_ShouldReturnCreatedEmail()
    {
        Server.StubJsonPost(OrgPath(EmailsPath), PingenResponseFactory.SingleEmail("email-created"));

        ApiResult<SingleResult<EmailDataDetailed>> result = await Client.Emails.Create(CreatePayload());

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe("email-created")
        );
    }

    [Test]
    public async Task Cancel_ShouldSucceed()
    {
        Server.StubJsonPatch(OrgPath($"{EmailsPath}/email-1/cancel"), "{}");

        ApiResult result = await Client.Emails.Cancel("email-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Delete_ShouldSucceed()
    {
        Server.StubDelete(OrgPath($"{EmailsPath}/email-1"));

        ApiResult result = await Client.Emails.Delete("email-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GetEventsPage_ShouldReturnEvents()
    {
        Server.StubJsonGet(OrgPath($"{EmailsPath}/email-1/events"), PingenResponseFactory.EmailEventCollection(emailId: "email-1"));

        ApiResult<CollectionResult<EmailEventData>> result = await Client.EmailEvents.GetEventsPage("email-1", "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3)
        );
    }

    [TestCase(401)]
    [TestCase(422)]
    [TestCase(500)]
    public async Task GetPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError(OrgPath(EmailsPath), "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<EmailData>> result = await Client.Emails.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull());
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldIterateAllEmails()
    {
        Server.StubJsonGet(OrgPath(EmailsPath), PingenResponseFactory.EmailCollection());

        var emails = new List<EmailData>();
        await foreach (var page in Client.Emails.GetPageResultsAsync())
            emails.AddRange(page);

        emails.Count.ShouldBe(3);
    }

    [Test]
    public async Task GetEventsPageResultsAsync_ShouldIterateAllEvents()
    {
        Server.StubJsonGet(OrgPath($"{EmailsPath}/email-1/events"), PingenResponseFactory.EmailEventCollection(emailId: "email-1"));

        var events = new List<EmailEventData>();
        await foreach (var page in Client.EmailEvents.GetEventsPageResultsAsync("email-1", "en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(3);
    }

    [Test]
    public async Task GetFileLocation_ShouldReturnLocation()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath($"{EmailsPath}/email-1/file")).UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(302)
                    .WithHeader("Location", "https://s3.example.com/email-1.pdf")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
            );

        ApiResult result = await Client.Emails.GetFileLocation("email-1");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Location.ShouldNotBeNull());
    }

    [Test]
    public async Task DownloadFileContent_ShouldReturnFileStream()
    {
        const string downloadPath = "/download/email-1.pdf";
        Server.Given(Request.Create().WithPath(downloadPath).UsingGet()).RespondWith(Response.Create().WithStatusCode(200).WithBody("%PDF-1.4 email"));

        await using Stream stream = await Client.Emails.DownloadFileContent(new Uri($"{Server.Url}{downloadPath}"));

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Length.ShouldBeGreaterThan(0);
    }
}

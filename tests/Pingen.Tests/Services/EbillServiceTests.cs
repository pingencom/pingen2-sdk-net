using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.EbillEvents;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Ebills.Embedded;
using Pingen.Abstractions.Models.Ebills.Views;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class EbillServiceTests : EndpointTestBase
{
    private const string EbillsPath = "deliveries/ebills";

    private static DataPost<EbillCreate, EbillCreateRelationships> CreatePayload() =>
        new()
        {
            Type = "ebills",
            Attributes = new EbillCreate
            {
                FileOriginalName = "invoice.pdf",
                FileUrl = "https://s3.example.com/upload/abc",
                FileUrlSignature = "signature-value",
                AutoSend = false,
                MetaData = new EbillMetaData
                {
                    InvoiceNumber = "INV-0001",
                    InvoiceDate = "2026-01-01",
                    InvoiceDueDate = "2026-01-31",
                    RecipientIdentifier = "41010560000000000",
                },
            },
            Relationships = EbillCreateRelationships.Create("preset-1"),
        };

    [Test]
    public async Task GetPage_ShouldReturnEbills()
    {
        Server.StubJsonGet(OrgPath(EbillsPath), PingenResponseFactory.EbillCollection());

        ApiResult<CollectionResult<EbillData>> result = await Client.Ebills.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.InvoiceNumber.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Get_ShouldReturnEbillDetail()
    {
        Server.StubJsonGet(OrgPath($"{EbillsPath}/ebill-1"), PingenResponseFactory.SingleEbill("ebill-1"));

        ApiResult<SingleResult<EbillDataDetailed>> result = await Client.Ebills.Get("ebill-1");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe("ebill-1"),
            () => result.Data!.Data.Meta.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task Create_ShouldReturnCreatedEbill()
    {
        Server.StubJsonPost(OrgPath(EbillsPath), PingenResponseFactory.SingleEbill("ebill-created"));

        ApiResult<SingleResult<EbillDataDetailed>> result = await Client.Ebills.Create(CreatePayload());

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe("ebill-created")
        );
    }

    [Test]
    public async Task Send_ShouldSucceed()
    {
        Server.StubJsonPatch(OrgPath($"{EbillsPath}/ebill-1/send"), "{}");

        ApiResult result = await Client.Ebills.Send("ebill-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Cancel_ShouldSucceed()
    {
        Server.StubJsonPatch(OrgPath($"{EbillsPath}/ebill-1/cancel"), "{}");

        ApiResult result = await Client.Ebills.Cancel("ebill-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Delete_ShouldSucceed()
    {
        Server.StubDelete(OrgPath($"{EbillsPath}/ebill-1"));

        ApiResult result = await Client.Ebills.Delete("ebill-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GetEventsPage_ShouldReturnEvents()
    {
        Server.StubJsonGet(OrgPath($"{EbillsPath}/ebill-1/events"), PingenResponseFactory.EbillEventCollection(ebillId: "ebill-1"));

        ApiResult<CollectionResult<EbillEventData>> result = await Client.EbillEvents.GetEventsPage("ebill-1", "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3)
        );
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldIterateAllEbills()
    {
        Server.StubJsonGet(OrgPath(EbillsPath), PingenResponseFactory.EbillCollection());

        var ebills = new List<EbillData>();
        await foreach (var page in Client.Ebills.GetPageResultsAsync())
            ebills.AddRange(page);

        ebills.Count.ShouldBe(3);
    }

    [Test]
    public async Task GetEventsPageResultsAsync_ShouldIterateAllEvents()
    {
        Server.StubJsonGet(OrgPath($"{EbillsPath}/ebill-1/events"), PingenResponseFactory.EbillEventCollection(ebillId: "ebill-1"));

        var events = new List<EbillEventData>();
        await foreach (var page in Client.EbillEvents.GetEventsPageResultsAsync("ebill-1", "en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(3);
    }

    [Test]
    public async Task GetFileLocation_ShouldReturnLocation()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath($"{EbillsPath}/ebill-1/file")).UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(302)
                    .WithHeader("Location", "https://s3.example.com/ebill-1.pdf")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
            );

        ApiResult result = await Client.Ebills.GetFileLocation("ebill-1");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Location.ShouldNotBeNull());
    }

    [Test]
    public async Task DownloadFileContent_ShouldReturnFileStream()
    {
        const string downloadPath = "/download/ebill-1.pdf";
        Server.Given(Request.Create().WithPath(downloadPath).UsingGet()).RespondWith(Response.Create().WithStatusCode(200).WithBody("%PDF-1.4 ebill"));

        await using Stream stream = await Client.Ebills.DownloadFileContent(new Uri($"{Server.Url}{downloadPath}"));

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Length.ShouldBeGreaterThan(0);
    }
}

using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Letters;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class PaginationTests : EndpointTestBase
{
    [Test]
    public async Task GetPageResultsAsync_EmptyFirstPage_ShouldYieldNoItemsAndStopAfterOneCall()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0, currentPage: 1, lastPage: 1));

        var allItems = new List<LetterData>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
            allItems.AddRange(page);

        allItems.ShouldBeEmpty();
        Server.VerifyCalled(OrgPath("deliveries/letters"));
    }

    [Test]
    public async Task GetPageResultsAsync_SinglePage_ShouldYieldAllItemsAndStopAfterOneCall()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection());

        var allItems = new List<LetterData>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
            allItems.AddRange(page);

        allItems.Count.ShouldBe(3);
        Server.VerifyCalled(OrgPath("deliveries/letters"));
    }

    [Test]
    public async Task GetPageResultsAsync_MultiPage_ShouldFetchAllPages()
    {
        const string scenario = "letters-multi-page";

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(2, currentPage: 1, lastPage: 3))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("page2")
            .WillSetStateTo("page3")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(2, currentPage: 2, lastPage: 3))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("page3")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(1, currentPage: 3, lastPage: 3))
            );

        var allItems = new List<LetterData>();
        int pageCount = 0;
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
        {
            pageCount++;
            allItems.AddRange(page);
        }

        pageCount.ShouldBe(3);
        allItems.Count.ShouldBe(5);
        Server.VerifyCalled(OrgPath("deliveries/letters"), times: 3);
    }

    [Test]
    public async Task GetPageResultsAsync_LastPageBoundary_ShouldNotRequestExtraPage()
    {
        const string scenario = "letters-boundary";
        const int perPage = 5;

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(perPage, currentPage: 1, lastPage: 2))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(perPage, currentPage: 2, lastPage: 2))
            );

        var allItems = new List<LetterData>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
            allItems.AddRange(page);

        allItems.Count.ShouldBe(perPage * 2);
        Server.VerifyCalled(OrgPath("deliveries/letters"), times: 2);
    }

    [Test]
    public async Task GetPageResultsAsync_StartingFromMiddlePage_ShouldFetchRemainingPages()
    {
        const string scenario = "letters-from-middle";

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WillSetStateTo("nextPage")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(2, currentPage: 2, lastPage: 3))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("nextPage")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(2, currentPage: 3, lastPage: 3))
            );

        var apiPagingRequest = new ApiPagingRequest { PageNumber = 2 };
        var allItems = new List<LetterData>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync(apiPagingRequest))
            allItems.AddRange(page);

        allItems.Count.ShouldBe(4);
        Server.VerifyCalled(OrgPath("deliveries/letters"), times: 2);
    }
}

using System.Net.Http.Headers;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Services.Connectors;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class ConcurrencyTests : EndpointTestBase
{
    [TearDown]
    public void TearDownClients()
    {
        foreach (HttpClient c in _ownedClients)
            c.Dispose();

        _ownedClients.Clear();
    }

    private readonly List<HttpClient> _ownedClients = [];

    [Test]
    public async Task MultipleClients_DifferentOrganisations_ShouldIsolateRequests()
    {
        const string orgA = "org-A";
        const string orgB = "org-B";

        Server.StubJsonGet($"/organisations/{orgA}/batches", PingenResponseFactory.BatchCollection(2, orgA));
        Server.StubJsonGet($"/organisations/{orgB}/batches", PingenResponseFactory.BatchCollection(5, orgB));

        IPingenApiClient clientA = CreateClient(orgA);
        IPingenApiClient clientB = CreateClient(orgB);

        Task<ApiResult<CollectionResult<BatchData>>> resultATask = clientA.Batches.GetPage();
        Task<ApiResult<CollectionResult<BatchData>>> resultBTask = clientB.Batches.GetPage();

        await Task.WhenAll(resultATask, resultBTask);

        ApiResult<CollectionResult<BatchData>> resultA = await resultATask;
        ApiResult<CollectionResult<BatchData>> resultB = await resultBTask;

        resultA.ShouldSatisfyAllConditions(
            () => resultA.IsSuccess.ShouldBeTrue(),
            () => resultA.Data.ShouldNotBeNull(),
            () => resultA.Data!.Data.Count.ShouldBe(2)
        );

        resultB.ShouldSatisfyAllConditions(
            () => resultB.IsSuccess.ShouldBeTrue(),
            () => resultB.Data.ShouldNotBeNull(),
            () => resultB.Data!.Data.Count.ShouldBe(5)
        );

        Server.VerifyCalled($"/organisations/{orgA}/batches");
        Server.VerifyCalled($"/organisations/{orgB}/batches");
    }

    [Test]
    public async Task SingleClient_ManyParallelRequests_ShouldAllSucceed()
    {
        IPingenApiClient freshClient = CreateClient(TestOrganisationId);

        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(1));

        Task<ApiResult<CollectionResult<BatchData>>>[] tasks = Enumerable.Range(0, 10).Select(_ => freshClient.Batches.GetPage()).ToArray();

        ApiResult<CollectionResult<BatchData>>[] results = await Task.WhenAll(tasks);

        results.Length.ShouldBe(10);
        foreach (ApiResult<CollectionResult<BatchData>> r in results)
        {
            r.IsSuccess.ShouldBeTrue();
            r.Data.ShouldNotBeNull();
        }

        Server.VerifyCalledAtLeastOnce(OrgPath("batches"));
    }

    private IPingenApiClient CreateClient(string organisationId)
    {
        string url = Server.Url!;

        var identityClient = new HttpClient { BaseAddress = new Uri(url) };
        identityClient.DefaultRequestHeaders.Accept.Clear();
        identityClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var apiClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { BaseAddress = new Uri(url) };

        var externalClient = new HttpClient { BaseAddress = new Uri(url) };

        _ownedClients.Add(identityClient);
        _ownedClients.Add(apiClient);
        _ownedClients.Add(externalClient);

        var configuration = new PingenConfiguration
        {
            BaseUri = "https://api.test.pingen.com/",
            IdentityUri = "https://identity.test.pingen.com/",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            DefaultOrganisationId = organisationId,
            MaxRetryAttempts = 1,
        };

        var httpClients = new PingenHttpClients(identityClient, apiClient, externalClient);
        var connectionHandler = new PingenConnectionHandler(configuration, httpClients);

        return new PingenApiClient(
            connectionHandler,
            new LetterService(connectionHandler),
            new BatchService(connectionHandler),
            new EmailService(connectionHandler),
            new EbillService(connectionHandler),
            new UserService(connectionHandler),
            new OrganisationService(connectionHandler),
            new WebhookService(connectionHandler),
            new FilesService(connectionHandler),
            new LetterEventService(connectionHandler),
            new BatchEventService(connectionHandler),
            new EmailEventService(connectionHandler),
            new EbillEventService(connectionHandler)
        );
    }
}

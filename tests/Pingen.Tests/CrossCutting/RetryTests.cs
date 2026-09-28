using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Interfaces;
using Pingen.Services;
using Pingen.Services.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class RetryTests : EndpointTestBase
{
    private readonly List<HttpClient> _ownedClients = [];

    [TearDown]
    public void DisposeOwnedClients()
    {
        foreach (var client in _ownedClients)
            client.Dispose();
        _ownedClients.Clear();
    }

    private IPingenApiClient CreateRetryClient(int maxRetryAttempts)
    {
        var url = Server.Url!;

        var identityClient = new HttpClient { BaseAddress = new Uri(url) };
        identityClient.DefaultRequestHeaders.Accept.Add(new("application/x-www-form-urlencoded"));
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
            DefaultOrganisationId = TestOrganisationId,
            MaxRetryAttempts = maxRetryAttempts,
        };

        var connectionHandler = new PingenConnectionHandler(configuration, new PingenHttpClients(identityClient, apiClient, externalClient));

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

    private void StubSequence(string scenario, int firstStatus, string firstBody)
    {
        Server
            .Given(Request.Create().WithPath(OrgPath("batches")).UsingGet())
            .InScenario(scenario)
            .WillSetStateTo("recovered")
            .RespondWith(
                Response.Create().WithStatusCode(firstStatus).WithHeader("Content-Type", "application/json").WithHeader("Retry-After", "0").WithBody(firstBody)
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("batches")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("recovered")
            .RespondWith(
                Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/json").WithBody(PingenResponseFactory.BatchCollection())
            );
    }

    [Test]
    public async Task GetPage_RetriesOn429ThenSucceeds()
    {
        StubSequence("retry-429", 429, PingenResponseFactory.ErrorResponse(status: "429"));

        ApiResult<CollectionResult<BatchData>> result = await CreateRetryClient(3).Batches.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }

    [Test]
    public async Task GetPage_RetriesOn503ThenSucceeds()
    {
        StubSequence("retry-503", 503, PingenResponseFactory.ErrorResponse(status: "503"));

        ApiResult<CollectionResult<BatchData>> result = await CreateRetryClient(3).Batches.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }

    [Test]
    public async Task GetPage_ReAuthenticatesOn401ThenSucceeds()
    {
        // First call returns 401 (stale token), the reactive refresh then lets the retry succeed.
        StubSequence("retry-401", 401, PingenResponseFactory.ErrorResponse(status: "401"));

        ApiResult<CollectionResult<BatchData>> result = await CreateRetryClient(3).Batches.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }

    [Test]
    public async Task GetPage_ExhaustsRetriesAndSurfacesError()
    {
        Server.StubError(OrgPath("batches"), "GET", PingenResponseFactory.ErrorResponse(status: "503"), 503);

        ApiResult<CollectionResult<BatchData>> result = await CreateRetryClient(2).Batches.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull());
    }

    [Test]
    public async Task GetPage_HonoursNumericRetryAfterHeader()
    {
        const string scenario = "retry-after-numeric";
        Server
            .Given(Request.Create().WithPath(OrgPath("batches")).UsingGet())
            .InScenario(scenario)
            .WillSetStateTo("recovered")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(429)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Retry-After", "1")
                    .WithBody(PingenResponseFactory.ErrorResponse(status: "429"))
            );
        Server
            .Given(Request.Create().WithPath(OrgPath("batches")).UsingGet())
            .InScenario(scenario)
            .WhenStateIs("recovered")
            .RespondWith(
                Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/json").WithBody(PingenResponseFactory.BatchCollection())
            );

        ApiResult<CollectionResult<BatchData>> result = await CreateRetryClient(3).Batches.GetPage();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }
}

using Pingen.Services.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Pingen.Tests.Helpers;

public abstract class EndpointTestBase
{
    protected const string TestOrganisationId = "test-org-id-001";

    protected WireMockServer Server = null!;

    protected IPingenApiClient Client = null!;

    private HttpClient _identityClient = null!;
    private HttpClient _apiClient = null!;
    private HttpClient _externalClient = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Server = WireMockServer.Start();
    }

    [SetUp]
    public void SetUp()
    {
        Server.Reset();
        StubTokenEndpoint();

        var wireMockUrl = Server.Url!;

        _identityClient = new HttpClient { BaseAddress = new Uri(wireMockUrl) };
        _identityClient.DefaultRequestHeaders.Accept.Clear();
        _identityClient.DefaultRequestHeaders.Accept.Add(new("application/x-www-form-urlencoded"));

        _apiClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { BaseAddress = new Uri(wireMockUrl) };

        _externalClient = new HttpClient { BaseAddress = new Uri(wireMockUrl) };

        var configuration = new PingenConfiguration
        {
            BaseUri = "https://api.test.pingen.com/",
            IdentityUri = "https://identity.test.pingen.com/",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            DefaultOrganisationId = TestOrganisationId,
            // Disable retry/backoff by default so error-path tests stay single-shot, fast and deterministic.
            // Retry behaviour is covered explicitly by RetryTests.
            MaxRetryAttempts = 1,
        };

        var httpClients = new PingenHttpClients(_identityClient, _apiClient, _externalClient);
        var connectionHandler = new PingenConnectionHandler(configuration, httpClients);

        Client = new PingenApiClient(
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

    [TearDown]
    public void TearDown()
    {
        _identityClient.Dispose();
        _apiClient.Dispose();
        _externalClient.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Server.Stop();
        Server.Dispose();
    }

    private void StubTokenEndpoint()
    {
        Server
            .Given(Request.Create().WithPath("/auth/access-tokens").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/json").WithBody(JsonApiStubHelper.TokenResponse()));
    }

    protected static string OrgPath(string path) => $"/organisations/{TestOrganisationId}/{path}";
}

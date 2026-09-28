using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Services.Connectors;

namespace Pingen.Tests.Integration;

public abstract class IntegrationSupport
{
    protected const string FileName = "test.pdf";

    protected const string FileNameCancellable = "test_simulate_cancellable.pdf";

    private static readonly string[] CredentialKeys = ["PINGEN2_CLIENT_ID", "PINGEN2_CLIENT_SECRET", "PINGEN2_ORGANIZATION_ID", "PINGEN2_ORGANIZATION_NAME"];

    private static IReadOnlyDictionary<string, string>? _credentials;

    protected IPingenApiClient Client { get; private set; } = null!;

    protected string OrganisationId { get; private set; } = null!;

    protected string? ExpectedOrganisationName { get; private set; }

    [OneTimeSetUp]
    public async Task IntegrationSetUp()
    {
        IReadOnlyDictionary<string, string> credentials = Credentials();

        if (string.IsNullOrEmpty(credentials["PINGEN2_CLIENT_ID"]) || string.IsNullOrEmpty(credentials["PINGEN2_CLIENT_SECRET"]))
            Assert.Ignore("Integration credentials not configured. Copy .env.example to .env and fill in PINGEN2_CLIENT_ID / PINGEN2_CLIENT_SECRET.");

        // Integration tests ALWAYS run against staging - never production. This is not configurable on purpose.
        ExpectedOrganisationName = string.IsNullOrEmpty(credentials["PINGEN2_ORGANIZATION_NAME"]) ? null : credentials["PINGEN2_ORGANIZATION_NAME"];

        var organisationId = credentials["PINGEN2_ORGANIZATION_ID"];
        Client = CreateClient(
            PingenConfiguration.Create(credentials["PINGEN2_CLIENT_ID"], credentials["PINGEN2_CLIENT_SECRET"], organisationId, useStaging: true)
        );

        // When no organisation is configured, use the first one the credentials can access.
        if (string.IsNullOrEmpty(organisationId))
        {
            ApiResult<CollectionResult<OrganisationData>> organisations = await Client.Organisations.GetPage();
            AssertSuccess(organisations);
            organisationId =
                organisations.Data!.Data.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No organisation available for the configured credentials.");
            Client.SetOrganisationId(organisationId);
        }

        OrganisationId = organisationId;
    }

    // Live, step-by-step console output (NUnit's Progress stream flushes immediately, so it shows during the run -
    // the .NET equivalent of PHPUnit's per-test testdox output the php suite prints). Run the suite with
    // `-l "console;verbosity=detailed"` to also get each test's pass/fail line from the runner.
    [SetUp]
    public void LogStepStart() => TestContext.Progress.WriteLine($"▶  {TestContext.CurrentContext.Test.Name}");

    [TearDown]
    public void LogStepResult()
    {
        TestContext context = TestContext.CurrentContext;
        var icon = context.Result.Outcome.Status.ToString() switch
        {
            "Passed" => "✔",
            "Failed" => "✘",
            "Skipped" => "○",
            _ => "·",
        };
        TestContext.Progress.WriteLine($"{icon}  {context.Test.Name}");
    }

    // Emit a step detail (created id, current state, ...) into the live progress stream.
    protected static void Log(string message) => TestContext.Progress.WriteLine($"   … {message}");

    private static IPingenApiClient CreateClient(IPingenConfiguration configuration)
    {
        var connectionHandler = new PingenConnectionHandler(configuration, PingenHttpClients.Create(configuration));

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

    private static IReadOnlyDictionary<string, string> Credentials()
    {
        if (_credentials is not null)
            return _credentials;

        Dictionary<string, string> dotenv = ParseDotenv(FindDotenv());
        var credentials = new Dictionary<string, string>();
        foreach (var key in CredentialKeys)
            credentials[key] = Environment.GetEnvironmentVariable(key) ?? (dotenv.TryGetValue(key, out var value) ? value : string.Empty);

        _credentials = credentials;
        return credentials;
    }

    private static string? FindDotenv()
    {
        for (DirectoryInfo? directory = new(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, ".env");
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    private static Dictionary<string, string> ParseDotenv(string? path)
    {
        var values = new Dictionary<string, string>();
        if (path is null || !File.Exists(path))
            return values;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            var separator = line.IndexOf('=');
            if (line.Length == 0 || line.StartsWith('#') || separator < 0)
                continue;

            values[line[..separator].Trim()] = line[(separator + 1)..].Trim().Trim('"', '\'');
        }

        return values;
    }

    protected static Stream OpenFixture(string fileName) => File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Assets", fileName));

    protected static async Task<T> PollUntil<T>(Func<Task<T>> fetch, Func<T, bool> until, int attempts = 20, int delayMilliseconds = 1500)
    {
        T result = await fetch();
        for (var attempt = 0; attempt < attempts && !until(result); attempt++)
        {
            await Task.Delay(delayMilliseconds);
            result = await fetch();
        }

        return result;
    }

    // Wait a fixed number of seconds for the API to move a delivery into the state the next step needs (mirrors the php suite).
    protected static async Task WaitFor(int seconds)
    {
        Log($"waiting {seconds}s for the API to advance the delivery");
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

    // Returns the id captured by an earlier ordered test, or skips the current test when that upstream create did not
    // run (e.g. it failed or was ignored). NUnit's [Order] only sequences tests; it has no skip-on-upstream-failure,
    // so without this a single create failure cascades into a wall of misleading downstream failures instead of the
    // clean skip php's #[Depends] produces.
    protected static string RequireCreated(string? id)
    {
        if (id is null)
            Assert.Ignore("Skipped: the upstream create test did not run.");

        return id!;
    }

    protected static void AssertSuccess(ApiResult result)
    {
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue("API ERROR: " + System.Text.Json.JsonSerializer.Serialize(result.ApiError));
    }

    protected static void AssertSuccess<T>(ApiResult<T> result)
        where T : class, IDataResult
    {
        AssertSuccess((ApiResult)result);
        result.Data.ShouldNotBeNull();
    }
}

# Pingen .NET SDK

The official .NET client library for the [Pingen API](https://api.pingen.com/documentation) - letters, batches, emails, e-bills, and more.

[![tests](https://github.com/pingencom/pingen2-sdk-net/actions/workflows/tests.yml/badge.svg)](https://github.com/pingencom/pingen2-sdk-net/actions/workflows/tests.yml)

| Package | Purpose |
|---|---|
| **Pingen** | The API client and services. |
| **Pingen.Abstractions** | Models, views and enums (referenced transitively). |
| **Pingen.AspNetCore** | Dependency-injection integration for ASP.NET Core. |

# Requirements

- .NET 10.0 or newer.
- A Pingen account and OAuth credentials for your desired grant type (usually `client_credentials`).

How to obtain these is described here: https://api.pingen.com/documentation#section/Authentication/How-to-obtain-a-Client-ID

# Installation

Add the packages from NuGet:

```bash
dotnet add package Pingen
dotnet add package Pingen.AspNetCore   # optional: ASP.NET Core dependency-injection integration
```

# Environments

We have two environments available: Production and Staging (see https://api.pingen.com/documentation#section/Basics/Environments).

This SDK supports staging as well. Pass `useStaging: true` when registering the client (see Usage); production is the default.

# Usage

The simplest way to integrate is using the client-credentials grant (see https://api.pingen.com/documentation#section/Authentication/Which-grant-type-should-i-use).

Register the client with dependency injection and inject `IPingenApiClient`:

```csharp
// Program.cs
builder.Services.AddPingenServices(
    clientId:              "YOUR_OAUTH2_CLIENT_ID",
    clientSecret:          "YOUR_OAUTH2_CLIENT_SECRET",
    defaultOrganisationId: "INSERT_YOUR_ORGANISATION_UUID_HERE",
    useStaging:            true);
```

```csharp
public sealed class LetterSender(IPingenApiClient pingen)
{
    public async Task SendAsync()
    {
        await using var pdf = File.OpenRead("your_original_pdf_name.pdf");

        await pingen.Letters.UploadAndCreate(pdf, new LetterUpload
        {
            FileOriginalName = "your_original_pdf_name.pdf",
            AddressPosition  = LetterAddressPosition.left,
            AutoSend         = false,
            DeliveryProduct  = LetterCreateDeliveryProduct.Cheap,
            PrintMode        = LetterPrintMode.simplex,
            PrintSpectrum    = LetterPrintSpectrum.color,
        });
    }
}
```

`UploadAndCreate` requests the file-upload URL, uploads the PDF, and creates the letter in one call. The same helper exists on `Batches`, `Emails` and `Ebills`. The OAuth access token is obtained, cached and refreshed automatically.

## Creating a batch (post, e-bill or email)

A batch dispatches through one channel. Set `ChannelType` on the batch to `post` (the default), `ebill` or `email`; the address and print options apply to postal batches only.

```csharp
public sealed class BatchSender(IPingenApiClient pingen)
{
    public async Task CreateEmailBatchAsync()
    {
        await using var pdf = File.OpenRead("mailing.pdf");

        ApiResult<SingleResult<BatchDataDetailed>> created = await pingen.Batches.UploadAndCreate(pdf, new BatchUpload
        {
            Name                     = "Invoices May 2026",
            ChannelType              = BatchChannelTypes.Email,   // or .Post / .Ebill
            Icon                     = BatchIcon.receipt,
            FileOriginalName         = "mailing.pdf",
            GroupingType             = BatchGroupingType.merge,
            GroupingOptionsSplitType = BatchGroupingOptionsSplitType.file,
        });

        // Send it. BatchSendRequest picks the channel-specific request type and attributes for you.
        await pingen.Batches.Send(BatchSendRequest.Email(created.Data!.Data.Id));
    }
}
```

For a postal batch, set `ChannelType = BatchChannelTypes.Post` (or omit it), add an `AddressPosition`, and send it with `BatchSendRequest.Post(batchId, deliveryProduct, printMode, printSpectrum)` - the `deliveryProduct` is one of the virtual products (`LetterCreateDeliveryProduct`, e.g. `cheap`/`fast`). E-bill batches send with `BatchSendRequest.Ebill(batchId)`.

Outside an ASP.NET Core host, build the configuration directly and wire the services yourself:

```csharp
var configuration = PingenConfiguration.Create(clientId, clientSecret, organisationId, useStaging: true);
```

# Examples & Docs

Our API docs are here: https://api.pingen.com/documentation

On the right-hand side of every endpoint you can see request samples for multiple languages. Each library method maps to one of these endpoints, so the docs are the reference for every attribute and delivery product.

# Bugreport & Contribution

If you find a bug, please either create a ticket in GitHub, or initiate a pull request. See [CONTRIBUTING.md](CONTRIBUTING.md) for the development setup.

# Versioning

We adhere to semantic (major.minor.patch) versioning (https://semver.org/). This means that:
* Patch (x.x.patch) versions fix bugs
* Minor (x.minor.x) versions introduce new, backwards compatible features or improve existing code.
* Major (major.x.x) versions introduce radical changes which are not backwards compatible.

In your automation or procedure you can always safely update patch & minor versions without the risk of your application failing.

# Testing

There are two test projects:

* **Pingen.Tests** - the offline suite (fast unit tests with mocked HTTP, plus endpoint tests driving a real client against an in-process mock HTTP server). No credentials required; this is what CI runs on every change.
* **Pingen.Tests.Integration** - talks to the **real Pingen Staging API**. Excluded from CI; skipped when credentials are absent.

Prepare the Docker image (no local .NET SDK required):

```bash
docker compose build
```

Running the offline tests (what CI runs):

```bash
docker compose run --rm dotnet-sdk dotnet test tests/Pingen.Tests
```

Running the integration tests (live calls against the Pingen staging API, credentials required in `.env`):

```bash
# Run the whole integration suite
docker compose run --rm dotnet-sdk dotnet test tests/Pingen.Tests.Integration

# Run one resource's integration tests
docker compose run --rm dotnet-sdk dotnet test tests/Pingen.Tests.Integration --filter "FullyQualifiedName~LettersIntegrationTests"
```

The suite reports progress live, step by step (each test as it starts/finishes, plus the waits it performs while the
API advances a delivery). Add `-l "console;verbosity=detailed"` to also print each test's pass/fail line from the runner:

```bash
docker compose run --rm dotnet-sdk dotnet test tests/Pingen.Tests.Integration -l "console;verbosity=detailed"
```

## Integration test credentials

Credentials come from a `.env` file in the repository root (copy `.env.example` and replace with your values), or from the same `PINGEN2_*` environment variables, which take precedence.

Without `PINGEN2_CLIENT_ID` / `PINGEN2_CLIENT_SECRET` the whole integration suite is **skipped** (not failed).

## Static analysis and style

`dotnet format analyzers` needs a restored workspace, so restore first (once) and pass `--no-restore`:

```bash
docker compose run --rm dotnet-sdk dotnet tool restore
docker compose run --rm dotnet-sdk dotnet restore Pingen.slnx
docker compose run --rm dotnet-sdk dotnet csharpier check .
docker compose run --rm dotnet-sdk dotnet format analyzers Pingen.slnx --verify-no-changes --no-restore
```

## Testing without Docker

We highly recommend using our prepared Docker image for running tests to ensure all necessary packages and versions are available. However, if you prefer, you can install a local .NET 10 SDK and run all commands without the `docker compose run --rm dotnet-sdk` prefix.

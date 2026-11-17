# Contributing

Thanks for your interest in contributing!

## Getting started

The project is developed inside Docker, so no local .NET SDK install is required. (You can of course use a
local .NET 10 SDK if you have one - the commands are the same without the `docker compose exec` prefix.)

```sh
git clone https://github.com/pingencom/pingen2-sdk-net.git
cd pingen2-sdk-net
docker compose build
docker compose up -d
docker compose exec dotnet-sdk dotnet tool restore
docker compose exec dotnet-sdk dotnet restore Pingen.slnx
docker compose exec dotnet-sdk dotnet test tests/Pingen.Tests
```

When you're done:

```sh
docker compose down
```

## Development workflow

1. Create a branch from `main`.
2. Make your changes.
3. Run all checks inside the container:
   ```sh
   docker compose exec dotnet-sdk dotnet restore Pingen.slnx                                             # restore (once)
   docker compose exec dotnet-sdk dotnet csharpier format .                                              # format
   docker compose exec dotnet-sdk dotnet csharpier check .                                               # format gate
   docker compose exec dotnet-sdk dotnet format analyzers Pingen.slnx --verify-no-changes --no-restore   # lint gate
   docker compose exec dotnet-sdk dotnet build Pingen.slnx -c Release --no-restore
   docker compose exec dotnet-sdk dotnet test tests/Pingen.Tests
   ```
4. Open a pull request.

## Code style

- **Formatting:** [CSharpier](https://csharpier.com/) - `dotnet csharpier format .` (config in `.csharpierrc.json`).
- **Linting:** Roslyn analyzers - `dotnet format analyzers Pingen.slnx --verify-no-changes --no-restore` (run `dotnet restore` first).
- Both are enforced in CI (`.github/workflows/tests.yml`); a PR that isn't formatted or has analyzer findings will fail.
- The build must be warning-free.

## Tests

- **Offline** (`tests/Pingen.Tests`) - the full offline suite, grouped by concern (`Services`, `Helpers`,
  `Models`, `Enums`, `Client`, `CrossCutting`, ...); service behaviour is exercised against a mocked HTTP server.
  Runs in CI and must stay green at 100% line coverage.
- **Integration** (`tests/Pingen.Tests.Integration`) tests run against the live **staging** environment and need
  credentials - see the "Running the integration suite against staging" section of the [README](README.md). Add
  or update integration coverage when you touch an endpoint's real request/response behaviour.

## API surface

Models are aligned property-by-property against the [Pingen API documentation](https://api.pingen.com/documentation).
When adding or changing an endpoint, verify the request/response shapes against the docs and the live API.

## Commit messages

Use clear, concise messages describing **what** changed and **why**.

## Reporting bugs

Open an issue using the bug report template.

using Microsoft.Extensions.DependencyInjection;
using Pingen.AspNetCore;
using Pingen.Interfaces.Connectors;

namespace Pingen.Tests.AspNetCore;

public class PingenServiceCollectionTests
{
    [Test]
    public void AddPingenServices_WithUseStagingOverload_RegistersClientForStaging()
    {
        var services = new ServiceCollection();

        services.AddPingenServices("test-client-id", "test-client-secret", "test-org-id", useStaging: true);

        var provider = services.BuildServiceProvider();
        provider.ShouldSatisfyAllConditions(
            () => provider.GetService<IPingenApiClient>().ShouldNotBeNull(),
            () => provider.GetRequiredService<IPingenConfiguration>().BaseUri.ShouldContain("api-staging.pingen.com"),
            () => provider.GetRequiredService<IPingenConfiguration>().IdentityUri.ShouldContain("identity-staging.pingen.com")
        );
    }

    [Test]
    public void AddPingenServices_RegistersAllServices()
    {
        var services = new ServiceCollection();

        services.AddPingenServices("https://api.example.com/", "https://identity.example.com/", "test-client-id", "test-client-secret", "test-org-id");

        services.ShouldSatisfyAllConditions(
            () => services.Any(s => s.ServiceType == typeof(IPingenConfiguration)).ShouldBeTrue("IPingenConfiguration not registered"),
            () => services.Any(s => s.ServiceType == typeof(IPingenConnectionHandler)).ShouldBeTrue("IPingenConnectionHandler not registered"),
            () => services.Any(s => s.ServiceType == typeof(IPingenApiClient)).ShouldBeTrue("IPingenApiClient not registered"),
            () => services.Any(s => s.ServiceType == typeof(ILetterService)).ShouldBeTrue("ILetterService not registered"),
            () => services.Any(s => s.ServiceType == typeof(IBatchService)).ShouldBeTrue("IBatchService not registered"),
            () => services.Any(s => s.ServiceType == typeof(IUserService)).ShouldBeTrue("IUserService not registered"),
            () => services.Any(s => s.ServiceType == typeof(IOrganisationService)).ShouldBeTrue("IOrganisationService not registered"),
            () => services.Any(s => s.ServiceType == typeof(IWebhookService)).ShouldBeTrue("IWebhookService not registered"),
            () => services.Any(s => s.ServiceType == typeof(IFilesService)).ShouldBeTrue("IFilesService not registered")
        );
    }

    [Test]
    public void AddPingenServices_RegistersCorrectLifetimes()
    {
        var services = new ServiceCollection();

        services.AddPingenServices("https://api.example.com/", "https://identity.example.com/", "test-client-id", "test-client-secret", "test-org-id");

        services.ShouldSatisfyAllConditions(
            () =>
                services
                    .First(s => s.ServiceType == typeof(IPingenConfiguration))
                    .Lifetime.ShouldBe(ServiceLifetime.Singleton, "IPingenConfiguration should be Singleton"),
            () =>
                services
                    .First(s => s.ServiceType == typeof(IPingenConnectionHandler))
                    .Lifetime.ShouldBe(ServiceLifetime.Scoped, "IPingenConnectionHandler should be Scoped"),
            () => services.First(s => s.ServiceType == typeof(IPingenApiClient)).Lifetime.ShouldBe(ServiceLifetime.Scoped, "IPingenApiClient should be Scoped"),
            () => services.First(s => s.ServiceType == typeof(ILetterService)).Lifetime.ShouldBe(ServiceLifetime.Scoped, "ILetterService should be Scoped")
        );
    }

    [Test]
    public void AddPingenServices_WithConfigObject_RegistersAllServices()
    {
        var services = new ServiceCollection();
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com/",
            IdentityUri = "https://identity.example.com/",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            DefaultOrganisationId = "test-org-id",
        };

        services.AddPingenServices(config);

        services.ShouldSatisfyAllConditions(
            () => services.Any(s => s.ServiceType == typeof(IPingenConfiguration)).ShouldBeTrue(),
            () => services.Any(s => s.ServiceType == typeof(IPingenApiClient)).ShouldBeTrue()
        );
    }

    [Test]
    public void AddPingenServices_CanResolveApiClient()
    {
        var services = new ServiceCollection();

        services.AddPingenServices("https://api.example.com/", "https://identity.example.com/", "test-client-id", "test-client-secret", "test-org-id");

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var client = scope.ServiceProvider.GetService<IPingenApiClient>();

        client.ShouldNotBeNull();
    }
}

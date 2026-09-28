namespace Pingen.Tests.Client;

[TestFixture]
public sealed class PingenHttpClientsTests
{
    [Test]
    public void Create_BuildsClientsForTheSelectedEnvironment()
    {
        IPingenConfiguration configuration = PingenConfiguration.Create("client-id", "client-secret", "org-id", useStaging: true);

        var clients = PingenHttpClients.Create(configuration);

        clients.ShouldSatisfyAllConditions(
            () => clients.Identity.BaseAddress!.ToString().ShouldContain("identity-staging.pingen.com"),
            () => clients.Api.BaseAddress!.ToString().ShouldContain("api-staging.pingen.com"),
            () => clients.External.ShouldNotBeNull()
        );
    }
}

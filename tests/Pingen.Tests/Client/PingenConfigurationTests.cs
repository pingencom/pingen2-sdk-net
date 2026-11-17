using Pingen.Services;

namespace Pingen.Tests.Client;

public class PingenConfigurationTests
{
    [Test]
    public void Create_Production_UsesProductionUrlsAndDefaults()
    {
        PingenConfiguration cfg = PingenConfiguration.Create("cid", "cs", "org");

        cfg.ShouldSatisfyAllConditions(
            () => cfg.BaseUri.ShouldBe(PingenEnvironment.ProductionBaseUri),
            () => cfg.IdentityUri.ShouldBe(PingenEnvironment.ProductionIdentityUri),
            () => cfg.ClientId.ShouldBe("cid"),
            () => cfg.ClientSecret.ShouldBe("cs"),
            () => cfg.DefaultOrganisationId.ShouldBe("org"),
            () => cfg.MaxRetryAttempts.ShouldBe(3)
        );
    }

    [Test]
    public void Create_Staging_UsesStagingUrls()
    {
        PingenConfiguration cfg = PingenConfiguration.Create("cid", "cs", "org", useStaging: true);

        cfg.ShouldSatisfyAllConditions(
            () => cfg.BaseUri.ShouldBe(PingenEnvironment.StagingBaseUri),
            () => cfg.IdentityUri.ShouldBe(PingenEnvironment.StagingIdentityUri)
        );
    }

    [Test]
    public void ToString_RedactsClientSecretAndSigningKeys()
    {
        var cfg = PingenConfiguration.Create("cid", "super-secret", "org") with
        {
            WebhookSigningKeys = new Dictionary<string, string> { ["wh-1"] = "signing-secret" },
        };

        var text = cfg.ToString();

        text.ShouldSatisfyAllConditions(
            () => text.ShouldNotContain("super-secret"),
            () => text.ShouldNotContain("signing-secret"),
            () => text.ShouldContain("ClientSecret = ***"),
            () => text.ShouldContain("WebhookSigningKeys = [1 redacted]"),
            () => text.ShouldContain("ClientId = cid")
        );
    }

    [Test]
    public void ToString_WithoutSigningKeys_DoesNotThrow()
    {
        var cfg = PingenConfiguration.Create("cid", "super-secret", "org");

        var text = cfg.ToString();

        text.ShouldSatisfyAllConditions(() => text.ShouldNotContain("super-secret"), () => text.ShouldContain("ClientSecret = ***"));
    }
}

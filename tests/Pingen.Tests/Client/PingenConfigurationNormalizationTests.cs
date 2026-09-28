namespace Pingen.Tests.Client;

public class PingenConfigurationNormalizationTests
{
    [Test]
    public void Normalize_BaseUriWithoutTrailingSlash_AppendsSlash()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com",
            IdentityUri = "https://identity.example.com",
            ClientId = "test",
            ClientSecret = "test",
            DefaultOrganisationId = "org",
        };

        var normalized = config.Normalize();

        normalized.ShouldSatisfyAllConditions(
            () => normalized.BaseUri.ShouldBe("https://api.example.com/"),
            () => normalized.IdentityUri.ShouldBe("https://identity.example.com/")
        );
    }

    [Test]
    public void Normalize_BaseUriWithTrailingSlash_NoChange()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com/",
            IdentityUri = "https://identity.example.com/",
            ClientId = "test",
            ClientSecret = "test",
            DefaultOrganisationId = "org",
        };

        var normalized = config.Normalize();

        normalized.ShouldSatisfyAllConditions(
            () => normalized.BaseUri.ShouldBe("https://api.example.com/"),
            () => normalized.IdentityUri.ShouldBe("https://identity.example.com/")
        );
    }

    [Test]
    public void Normalize_HttpBaseUri_ThrowsArgumentException()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "http://api.example.com",
            IdentityUri = "https://identity.example.com",
            ClientId = "test",
            ClientSecret = "test",
            DefaultOrganisationId = "org",
        };

        Should.Throw<ArgumentException>(() => config.Normalize()).Message.ShouldContain("BaseUri");
    }

    [Test]
    public void Normalize_HttpIdentityUri_ThrowsArgumentException()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com",
            IdentityUri = "http://identity.example.com",
            ClientId = "test",
            ClientSecret = "test",
            DefaultOrganisationId = "org",
        };

        Should.Throw<ArgumentException>(() => config.Normalize()).Message.ShouldContain("IdentityUri");
    }

    [Test]
    public void Normalize_HttpsUris_DoesNotThrow()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com",
            IdentityUri = "https://identity.example.com",
            ClientId = "test",
            ClientSecret = "test",
            DefaultOrganisationId = "org",
        };

        Should.NotThrow(() => config.Normalize());
    }

    [Test]
    public void PingenConfiguration_StoresAllProperties()
    {
        var config = new PingenConfiguration
        {
            BaseUri = "https://api.example.com/",
            IdentityUri = "https://identity.example.com/",
            ClientId = "my-client-id",
            ClientSecret = "my-client-secret",
            DefaultOrganisationId = "my-org-id",
            WebhookSigningKeys = new Dictionary<string, string> { ["key1"] = "secret1" },
        };

        config.ShouldSatisfyAllConditions(
            () => config.BaseUri.ShouldBe("https://api.example.com/"),
            () => config.IdentityUri.ShouldBe("https://identity.example.com/"),
            () => config.ClientId.ShouldBe("my-client-id"),
            () => config.ClientSecret.ShouldBe("my-client-secret"),
            () => config.DefaultOrganisationId.ShouldBe("my-org-id"),
            () => config.WebhookSigningKeys.Count.ShouldBe(1)
        );
    }
}

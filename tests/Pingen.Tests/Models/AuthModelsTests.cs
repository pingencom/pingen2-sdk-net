using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Models;

namespace Pingen.Tests.Models;

[TestFixture]
public sealed class AuthModelsTests
{
    [Test]
    public void AccessToken_ExposesAllPropertiesAndComputesExpiry()
    {
        var token = new AccessToken("token-value", "Bearer", 3600);

        token.ShouldSatisfyAllConditions(
            () => token.Token.ShouldBe("token-value"),
            () => token.TokenType.ShouldBe("Bearer"),
            () => token.ExpiresIn.ShouldBe(3600),
            () => token.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow),
            () => token.ExpiresAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddSeconds(3600))
        );
    }

    [Test]
    public void AccessToken_ToString_RedactsTheRawToken()
    {
        var token = new AccessToken("super-secret-token", "Bearer", 3600);

        var text = token.ToString();

        text.ShouldSatisfyAllConditions(
            () => text.ShouldNotContain("super-secret-token"),
            () => text.ShouldContain("Token = ***"),
            () => text.ShouldContain("Bearer")
        );
    }

    [Test]
    public void PingenApiAuthError_ExposesAllProperties()
    {
        var error = new PingenApiAuthError("invalid_client", "Client authentication failed", "Unauthorized");

        error.ShouldSatisfyAllConditions(
            () => error.Error.ShouldBe("invalid_client"),
            () => error.ErrorDescription.ShouldBe("Client authentication failed"),
            () => error.Message.ShouldBe("Unauthorized")
        );
    }
}

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class AuthenticationIntegrationTests : IntegrationSupport
{
    [Test]
    public async Task Authenticates_AndFetchesCurrentUser()
    {
        var result = await Client.Users.Get();

        AssertSuccess(result);
    }
}

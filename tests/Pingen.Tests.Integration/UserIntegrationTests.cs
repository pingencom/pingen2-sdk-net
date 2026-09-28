namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class UserIntegrationTests : IntegrationSupport
{
    [Test]
    [Order(1)]
    public async Task GetUser()
    {
        var user = await Client.Users.Get();
        AssertSuccess(user);
        user.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        user.Data.Data.Attributes.Email.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(2)]
    public async Task GetUserAssociations()
    {
        var associations = await Client.Users.GetAssociationsPage();
        AssertSuccess(associations);
    }
}

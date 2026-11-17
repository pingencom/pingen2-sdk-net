using Pingen.Abstractions.Models.Api;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class OrganisationsIntegrationTests : IntegrationSupport
{
    [Test]
    [Order(1)]
    public async Task ListOrganisations()
    {
        var collection = await Client.Organisations.GetPage();
        AssertSuccess(collection);
        collection.Data!.Data.Count.ShouldBeGreaterThanOrEqualTo(1);

        foreach (var organisation in collection.Data.Data)
            organisation.Id.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(2)]
    public async Task ListOrganisationsPaginated()
    {
        var collection = await Client.Organisations.GetPage(new ApiPagingRequest { PageNumber = 1, PageLimit = 5 });
        AssertSuccess(collection);
        collection.Data!.Data.Count.ShouldBeLessThanOrEqualTo(5);
        collection.Data.Meta.CurrentPage.ShouldBe(1);
    }

    [Test]
    [Order(3)]
    public async Task GetOrganisationById()
    {
        var details = await Client.Organisations.Get(OrganisationId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(OrganisationId);
    }
}

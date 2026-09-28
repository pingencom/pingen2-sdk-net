using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class OrganisationServiceTests : EndpointTestBase
{
    [Test]
    public async Task GetPage_ShouldReturnOrganisations()
    {
        Server.StubJsonGet("/organisations", PingenResponseFactory.OrganisationCollection());

        ApiResult<CollectionResult<OrganisationData>> result = await Client.Organisations.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Name.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task GetPage_ShouldReturnEmptyWhenNoOrganisations()
    {
        Server.StubJsonGet("/organisations", PingenResponseFactory.OrganisationCollection(0));

        ApiResult<CollectionResult<OrganisationData>> result = await Client.Organisations.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet("/organisations", PingenResponseFactory.OrganisationCollection(5, 2, 3));

        ApiResult<CollectionResult<OrganisationData>> result = await Client.Organisations.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(2),
            () => result.Data!.Meta.LastPage.ShouldBe(3),
            () => result.Data!.Data.Count.ShouldBe(5)
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task GetPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError("/organisations", "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<OrganisationData>> result = await Client.Organisations.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldAutoPaginate()
    {
        Server
            .Given(Request.Create().WithPath("/organisations").UsingGet())
            .InScenario("org-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.OrganisationCollection(1, 1, 2))
            );

        Server
            .Given(Request.Create().WithPath("/organisations").UsingGet())
            .InScenario("org-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.OrganisationCollection(1, 2, 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<OrganisationData> page in Client.Organisations.GetPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldYieldSinglePage_WhenOnlyOneExists()
    {
        Server.StubJsonGet("/organisations", PingenResponseFactory.OrganisationCollection(2));

        var allItems = new List<string>();
        await foreach (IEnumerable<OrganisationData> page in Client.Organisations.GetPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        Server.StubError("/organisations", "GET", PingenResponseFactory.ErrorResponse("Server Error", "Service unavailable", "500"), 500);

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<OrganisationData> _ in Client.Organisations.GetPageResultsAsync())
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task Get_ShouldReturnSingleOrganisation()
    {
        string orgId = Guid.NewGuid().ToString();

        Server.StubJsonGet($"/organisations/{orgId}", PingenResponseFactory.SingleOrganisation(orgId));

        ApiResult<SingleResult<OrganisationDataDetailed>> result = await Client.Organisations.Get(orgId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(orgId),
            () => result.Data!.Data.Attributes.Name.ShouldNotBeNullOrEmpty(),
            () => result.Data!.Data.Attributes.BillingCurrency.ShouldBe("CHF")
        );
    }

    [Test]
    public async Task Get_ShouldReturnErrorWhenNotFound()
    {
        string orgId = Guid.NewGuid().ToString();

        Server.StubError(
            $"/organisations/{orgId}",
            "GET",
            PingenResponseFactory.ErrorResponse("Not Found", "The requested organisation does not exist.", "404"),
            404
        );

        ApiResult<SingleResult<OrganisationDataDetailed>> result = await Client.Organisations.Get(orgId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found"),
            () => result.Data.ShouldBeNull()
        );
    }
}

using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Users;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class UserServiceTests : EndpointTestBase
{
    [Test]
    public async Task Get_ShouldReturnAuthenticatedUser()
    {
        string userId = Guid.NewGuid().ToString();

        Server.StubJsonGet("/user", PingenResponseFactory.SingleUser(userId));

        ApiResult<SingleResult<UserDataDetailed>> result = await Client.Users.Get();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(userId),
            () => result.Data!.Data.Attributes.Email.ShouldNotBeNullOrEmpty(),
            () => result.Data!.Data.Attributes.FirstName.ShouldNotBeNullOrEmpty(),
            () => result.Data!.Data.Attributes.LastName.ShouldNotBeNullOrEmpty()
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task Get_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError("/user", "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<SingleResult<UserDataDetailed>> result = await Client.Users.Get();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetAssociationsPage_ShouldReturnAssociations()
    {
        Server.StubJsonGet("/user/associations", PingenResponseFactory.UserAssociationCollection());

        ApiResult<CollectionResult<UserAssociationDataDetailed>> result = await Client.Users.GetAssociationsPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Role.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task GetAssociationsPage_ShouldReturnEmptyWhenNoAssociations()
    {
        Server.StubJsonGet("/user/associations", PingenResponseFactory.UserAssociationCollection(0));

        ApiResult<CollectionResult<UserAssociationDataDetailed>> result = await Client.Users.GetAssociationsPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetAssociationsPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet("/user/associations", PingenResponseFactory.UserAssociationCollection(4, 1, 3));

        ApiResult<CollectionResult<UserAssociationDataDetailed>> result = await Client.Users.GetAssociationsPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(1),
            () => result.Data!.Meta.LastPage.ShouldBe(3),
            () => result.Data!.Data.Count.ShouldBe(4)
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task GetAssociationsPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError("/user/associations", "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<UserAssociationDataDetailed>> result = await Client.Users.GetAssociationsPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetAssociationsPageResultsAsync_ShouldAutoPaginate()
    {
        Server
            .Given(Request.Create().WithPath("/user/associations").UsingGet())
            .InScenario("user-assoc-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.UserAssociationCollection(1, 1, 2))
            );

        Server
            .Given(Request.Create().WithPath("/user/associations").UsingGet())
            .InScenario("user-assoc-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.UserAssociationCollection(1, 2, 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<UserAssociationDataDetailed> page in Client.Users.GetAssociationsPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetAssociationsPageResultsAsync_ShouldYieldSinglePage_WhenOnlyOneExists()
    {
        Server.StubJsonGet("/user/associations", PingenResponseFactory.UserAssociationCollection(2));

        var allItems = new List<string>();
        await foreach (IEnumerable<UserAssociationDataDetailed> page in Client.Users.GetAssociationsPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetAssociationsPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        Server.StubError("/user/associations", "GET", PingenResponseFactory.ErrorResponse("Forbidden", "Access denied", "403"), 403);

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<UserAssociationDataDetailed> _ in Client.Users.GetAssociationsPageResultsAsync())
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }
}

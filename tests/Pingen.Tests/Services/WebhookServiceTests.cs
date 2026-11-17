using Bogus;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.Views;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class WebhookServiceTests : EndpointTestBase
{
    private static Faker<DataPost<WebhookCreate>> WebhookCreateFaker()
    {
        return new Faker<DataPost<WebhookCreate>>()
            .RuleFor(x => x.Type, "webhooks")
            .RuleFor(
                x => x.Attributes,
                f => new WebhookCreate
                {
                    EventCategory = f.PickRandom(
                        WebhookEventCategory.Issues,
                        WebhookEventCategory.Undeliverable,
                        WebhookEventCategory.Sent,
                        WebhookEventCategory.Delivered
                    ),
                    Url = new Uri(f.Internet.UrlWithPath()),
                    SigningKey = f.Random.AlphaNumeric(32),
                }
            );
    }

    [Test]
    public async Task GetPage_ShouldReturnWebhooks()
    {
        Server.StubJsonGet(OrgPath("webhooks"), PingenResponseFactory.WebhookCollection());

        ApiResult<CollectionResult<WebhookData>> result = await Client.Webhooks.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Url.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task GetPage_ShouldReturnEmptyWhenNoWebhooks()
    {
        Server.StubJsonGet(OrgPath("webhooks"), PingenResponseFactory.WebhookCollection(0));

        ApiResult<CollectionResult<WebhookData>> result = await Client.Webhooks.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet(OrgPath("webhooks"), PingenResponseFactory.WebhookCollection(4, currentPage: 1, lastPage: 3));

        ApiResult<CollectionResult<WebhookData>> result = await Client.Webhooks.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(1),
            () => result.Data!.Meta.LastPage.ShouldBe(3)
        );
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldAutoPaginate()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath("webhooks")).UsingGet())
            .InScenario("webhooks-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.WebhookCollection(1, currentPage: 1, lastPage: 2))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("webhooks")).UsingGet())
            .InScenario("webhooks-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.WebhookCollection(1, currentPage: 2, lastPage: 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<WebhookData> page in Client.Webhooks.GetPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        Server.StubError(OrgPath("webhooks"), "GET", PingenResponseFactory.ErrorResponse("Forbidden", "Access denied", "403"), 403);

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<WebhookData> _ in Client.Webhooks.GetPageResultsAsync())
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task Create_ShouldPostWebhookAndReturnResult()
    {
        string webhookId = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("webhooks"), PingenResponseFactory.SingleWebhook(webhookId));

        DataPost<WebhookCreate>? data = WebhookCreateFaker().Generate();

        ApiResult<SingleResult<WebhookData>> result = await Client.Webhooks.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(webhookId),
            () => result.Data!.Data.Attributes.Url.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task Create_ShouldReturnErrorWhenValidationFails()
    {
        Server.StubError(OrgPath("webhooks"), "POST", PingenResponseFactory.ErrorResponse("Validation Failed", "URL is required."));

        DataPost<WebhookCreate>? data = WebhookCreateFaker().Generate();

        ApiResult<SingleResult<WebhookData>> result = await Client.Webhooks.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Validation Failed"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Get_ShouldReturnSingleWebhook()
    {
        string webhookId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"webhooks/{webhookId}"), PingenResponseFactory.SingleWebhook(webhookId));

        ApiResult<SingleResult<WebhookData>> result = await Client.Webhooks.Get(webhookId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(webhookId),
            () => result.Data!.Data.Attributes.Url.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task Get_ShouldReturnErrorWhenNotFound()
    {
        string webhookId = Guid.NewGuid().ToString();

        Server.StubError(OrgPath($"webhooks/{webhookId}"), "GET", PingenResponseFactory.ErrorResponse("Not Found", "The webhook does not exist.", "404"), 404);

        ApiResult<SingleResult<WebhookData>> result = await Client.Webhooks.Get(webhookId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Delete_ShouldReturnSuccess()
    {
        string webhookId = Guid.NewGuid().ToString();

        Server.StubDelete(OrgPath($"webhooks/{webhookId}"));

        ApiResult result = await Client.Webhooks.Delete(webhookId);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Delete_ShouldReturnErrorWhenNotFound()
    {
        string webhookId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"webhooks/{webhookId}"),
            "DELETE",
            PingenResponseFactory.ErrorResponse("Not Found", "The webhook does not exist.", "404"),
            404
        );

        ApiResult result = await Client.Webhooks.Delete(webhookId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found")
        );
    }
}

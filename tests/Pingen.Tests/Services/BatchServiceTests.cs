using Bogus;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Batches.Views;
using Pingen.Abstractions.Models.BatchEvents;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class BatchServiceTests : EndpointTestBase
{
    private static Faker<DataPost<BatchCreate, BatchCreateRelationships>> BatchCreateFaker()
    {
        return new Faker<DataPost<BatchCreate, BatchCreateRelationships>>()
            .RuleFor(x => x.Type, "batches")
            .RuleFor(
                x => x.Attributes,
                f => new BatchCreate
                {
                    Name = f.Commerce.ProductName(),
                    Icon = f.PickRandom<BatchIcon>(),
                    FileOriginalName = f.System.FileName("pdf"),
                    FileUrl = f.Internet.Url(),
                    FileUrlSignature = f.Random.Hash(),
                    AddressPosition = f.PickRandom<LetterAddressPosition>(),
                    GroupingType = f.PickRandom<BatchGroupingType>(),
                    GroupingOptionsSplitType = f.PickRandom<BatchGroupingOptionsSplitType>(),
                }
            )
            .RuleFor(x => x.Relationships, f => BatchCreateRelationships.Create(f.Random.Guid().ToString()));
    }

    [Test]
    public async Task Send_WithEmptyType_ThrowsArgumentException()
    {
        // A hand-built DataPatch that skipped the BatchSendRequest factory has no channel-specific type; the
        // service must reject it client-side instead of letting it become a confusing server-side 4xx.
        var data = new DataPatch<BatchSend>
        {
            Id = "batch-1",
            Attributes = new BatchSend { DeliveryProduct = "cheap" },
        };

        await Should.ThrowAsync<ArgumentException>(async () => await Client.Batches.Send(data));
    }

    [Test]
    public async Task GetPage_ShouldReturnBatches()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection());

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Name.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task GetPage_ShouldReturnEmptyWhenNoBatches()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(0));

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task GetPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError(OrgPath("batches"), "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Get_ShouldReturnSingleBatch()
    {
        string batchId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"batches/{batchId}"), PingenResponseFactory.SingleBatch(batchId));

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Get(batchId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(batchId),
            () => result.Data!.Data.Attributes.Name.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Get_ShouldReturnErrorWhenNotFound()
    {
        string batchId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"batches/{batchId}"),
            "GET",
            PingenResponseFactory.ErrorResponse("Not Found", "The requested batch does not exist.", "404"),
            404
        );

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Get(batchId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Create_ShouldPostBatchAndReturnResult()
    {
        string batchId = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("batches"), PingenResponseFactory.SingleBatch(batchId));

        DataPost<BatchCreate, BatchCreateRelationships>? data = BatchCreateFaker().Generate();

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(batchId),
            () => result.Data!.Data.Attributes.Name.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Create_ShouldReturnErrorWhenValidationFails()
    {
        Server.StubError(OrgPath("batches"), "POST", PingenResponseFactory.ErrorResponse("Validation Failed", "Name is required."));

        DataPost<BatchCreate, BatchCreateRelationships>? data = BatchCreateFaker().Generate();

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Validation Failed"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(5, currentPage: 2, lastPage: 3));

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(2),
            () => result.Data!.Meta.LastPage.ShouldBe(3),
            () => result.Data!.Data.Count.ShouldBe(5)
        );
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldIterateAllBatches()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection());

        var batches = new List<BatchData>();
        await foreach (var page in Client.Batches.GetPageResultsAsync())
            batches.AddRange(page);

        batches.Count.ShouldBe(3);
    }

    [Test]
    public async Task GetEventsPageResultsAsync_ShouldIterateAllEvents()
    {
        Server.StubJsonGet(OrgPath("batches/batch-1/events"), PingenResponseFactory.BatchEventCollection(batchId: "batch-1"));

        var events = new List<BatchEventData>();
        await foreach (var page in Client.BatchEvents.GetEventsPageResultsAsync("batch-1", "en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(3);
    }
}

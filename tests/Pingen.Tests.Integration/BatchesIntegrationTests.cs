using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Batches.Views;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class BatchesIntegrationTests : IntegrationSupport
{
    private string _batchId = null!;

    private string _deletionBatchId = null!;

    [Test]
    [Order(1)]
    public async Task ListBatches()
    {
        var collection = await Client.Batches.GetPage();
        AssertSuccess(collection);
    }

    [Test]
    [Order(2)]
    public async Task ListBatchesPaginated()
    {
        var collection = await Client.Batches.GetPage(new ApiPagingRequest { PageNumber = 1, PageLimit = 3 });
        AssertSuccess(collection);
        collection.Data!.Data.Count.ShouldBeLessThanOrEqualTo(3);
    }

    [Test]
    [Order(3)]
    public async Task CreateBatch()
    {
        var created = await Client.Batches.UploadAndCreate(OpenFixture(FileName), Batch(FileName));
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.ChannelType.ShouldBe("post");
        _batchId = created.Data.Data.Id;
    }

    [Test]
    [Order(4)]
    public async Task GetBatchById()
    {
        _ = RequireCreated(_batchId);

        var details = await Client.Batches.Get(_batchId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_batchId);
        details.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(5)]
    public async Task UpdateBatch()
    {
        _ = RequireCreated(_batchId);

        await WaitFor(10);

        var edited = await Client.Batches.Update(
            new DataPatch<BatchEdit>
            {
                Id = _batchId,
                Type = "batches",
                Attributes = new BatchEdit { Name = "Updated Integration Batch", Icon = BatchIcon.rocket },
            }
        );
        AssertSuccess(edited);
        edited.Data!.Data.Attributes.Name.ShouldBe("Updated Integration Batch");
    }

    [Test]
    [Order(6)]
    public async Task GetBatchEvents()
    {
        _ = RequireCreated(_batchId);

        var events = await Client.BatchEvents.GetEventsPage(_batchId, "en-GB");
        AssertSuccess(events);
    }

    [Test]
    [Order(7)]
    public async Task GetBatchStatistics()
    {
        _ = RequireCreated(_batchId);

        var statistics = await Client.Batches.GetStatistics(_batchId);
        AssertSuccess(statistics);
        statistics.Data!.Data.Id.ShouldBe(_batchId);
    }

    [Test]
    [Order(8)]
    public async Task CreateBatchForDeletion()
    {
        var created = await Client.Batches.UploadAndCreate(OpenFixture(FileNameCancellable), Batch(FileNameCancellable));
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        _deletionBatchId = created.Data.Data.Id;
    }

    [Test]
    [Order(9)]
    public async Task DeleteBatch()
    {
        _ = RequireCreated(_deletionBatchId);

        // Give the batch time to reach a deletable state.
        await WaitFor(10);

        var deleted = await Client.Batches.Delete(_deletionBatchId, withDeliverables: true);
        AssertSuccess(deleted);

        var afterDelete = await Client.Batches.Get(_deletionBatchId);
        afterDelete.IsSuccess.ShouldBeFalse();
    }

    private static BatchUpload Batch(string fileName) =>
        new()
        {
            Name = "Integration Test Batch",
            ChannelType = BatchChannelTypes.Post,
            Icon = BatchIcon.document,
            FileOriginalName = fileName,
            AddressPosition = LetterAddressPosition.left,
            GroupingType = BatchGroupingType.merge,
            GroupingOptionsSplitType = BatchGroupingOptionsSplitType.qr_invoice,
            GroupingOptionsSplitSize = 2,
        };
}

using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Batches.Statistics;
using Pingen.Abstractions.Models.Batches.Views;
using Pingen.Abstractions.Models.BatchEvents;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class BatchLifecycleServiceTests : EndpointTestBase
{
    [Test]
    public async Task Send_ShouldReturnUpdatedBatch()
    {
        Server.StubJsonPatch(OrgPath("batches/batch-1/send"), PingenResponseFactory.SingleBatch("batch-1"));

        DataPatch<BatchSend> data = BatchSendRequest.Post("batch-1", "fast", LetterPrintMode.simplex, LetterPrintSpectrum.color);

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Send(data);

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("batch-1"));
    }

    [Test]
    public async Task Send_EmailBatch_ShouldReturnUpdatedBatch()
    {
        Server.StubJsonPatch(OrgPath("batches/batch-email/send"), PingenResponseFactory.SingleBatch("batch-email"));

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Send(BatchSendRequest.Email("batch-email"));

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("batch-email"));
    }

    [Test]
    public async Task Cancel_ShouldSucceed()
    {
        Server.StubJsonPatch(OrgPath("batches/batch-1/cancel"), "{}");

        ApiResult result = await Client.Batches.Cancel("batch-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Delete_ShouldSucceed()
    {
        Server.StubDelete(OrgPath("batches/batch-1"));

        ApiResult result = await Client.Batches.Delete("batch-1");

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Update_ShouldReturnUpdatedBatch()
    {
        Server.StubJsonPatch(OrgPath("batches/batch-1"), PingenResponseFactory.SingleBatch("batch-1"));

        var data = new DataPatch<BatchEdit>
        {
            Id = "batch-1",
            Type = "batches",
            Attributes = new BatchEdit { Name = "Renamed batch", Icon = BatchIcon.rocket },
        };

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Update(data);

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Id.ShouldBe("batch-1"));
    }

    [Test]
    public async Task GetStatistics_ShouldReturnStatistics()
    {
        Server.StubJsonGet(OrgPath("batches/batch-1/statistics"), PingenResponseFactory.SingleBatchStatistics("batch-1"));

        ApiResult<SingleResult<BatchStatisticsData>> result = await Client.Batches.GetStatistics("batch-1");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Attributes.LetterGroups.ShouldNotBeNull()
        );
    }

    [Test]
    public async Task GetEventsPage_ShouldReturnBatchEvents()
    {
        Server.StubJsonGet(OrgPath("batches/batch-1/events"), PingenResponseFactory.BatchEventCollection(batchId: "batch-1"));

        ApiResult<CollectionResult<BatchEventData>> result = await Client.BatchEvents.GetEventsPage("batch-1", "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Code.ShouldNotBeNullOrEmpty()
        );
    }
}

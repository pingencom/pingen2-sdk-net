using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Batches.Statistics;
using Pingen.Abstractions.Models.Batches.Views;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IBatchService" />
public sealed class BatchService : ConnectorService, IBatchService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchService"/> class.
    /// </summary>
    public BatchService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<BatchData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<BatchData>>(BatchesEndpoints.Root, apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchDataDetailed>>> Create(
        DataPost<BatchCreate, BatchCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "batches" };

        return await ConnectionHandler.PostAsync<SingleResult<BatchDataDetailed>, DataPost<BatchCreate, BatchCreateRelationships>>(
            requestPath: BatchesEndpoints.Root,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchDataDetailed>>> Get(
        string batchId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<BatchDataDetailed>>(
            requestPath: BatchesEndpoints.Single(batchId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<BatchData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetPage(apiRequest, cancellationToken)).WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchDataDetailed>>> Send(
        DataPatch<BatchSend> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        // Batch send is channel-polymorphic (batches_channel_{post|email|ebill}_send), so - unlike the other
        // batch operations - the service cannot force a single type. Guard against a hand-built request that
        // skipped BatchSendRequest, turning a confusing server-side 4xx into a clear client-side error.
        if (string.IsNullOrEmpty(data.Type))
            throw new ArgumentException(
                $"{nameof(DataPatch<BatchSend>)}.{nameof(data.Type)} is required; use a BatchSendRequest factory method.",
                nameof(data)
            );

        return await ConnectionHandler.PatchAsync<SingleResult<BatchDataDetailed>, DataPatch<BatchSend>>(
            BatchesEndpoints.Send(data.Id),
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Cancel(string batchId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.PatchAsync(BatchesEndpoints.Cancel(batchId), idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> Delete(string batchId, [Optional] bool withDeliverables, [Optional] CancellationToken cancellationToken)
    {
        var data = new DataPatch<BatchDelete>
        {
            Id = batchId,
            Type = "batches",
            Attributes = new BatchDelete { WithLetters = withDeliverables, WithDeliverables = withDeliverables },
        };

        return await ConnectionHandler.DeleteAsync(BatchesEndpoints.Single(batchId), data, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchDataDetailed>>> Update(
        DataPatch<BatchEdit> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "batches" };

        return await ConnectionHandler.PatchAsync<SingleResult<BatchDataDetailed>, DataPatch<BatchEdit>>(
            BatchesEndpoints.Single(data.Id),
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchStatisticsData>>> GetStatistics(
        string batchId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<BatchStatisticsData>>(
            requestPath: BatchesEndpoints.Statistics(batchId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> AddAttachment(
        string batchId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "batches" };

        return await ConnectionHandler.PatchAsync(BatchesEndpoints.Attachment(batchId), data, idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<BatchDataDetailed>>> UploadAndCreate(
        Stream file,
        BatchUpload attributes,
        [Optional] BatchCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        FileUploadData fileUploadData = await RequestAndUploadFile(file, cancellationToken);

        return await Create(
            new DataPost<BatchCreate, BatchCreateRelationships>
            {
                Type = "batches",
                Attributes = new BatchCreate
                {
                    Name = attributes.Name,
                    ChannelType = attributes.ChannelType,
                    Icon = attributes.Icon,
                    FileOriginalName = attributes.FileOriginalName,
                    FileUrl = fileUploadData.Attributes.Url,
                    FileUrlSignature = fileUploadData.Attributes.UrlSignature,
                    AddressPosition = attributes.AddressPosition,
                    GroupingType = attributes.GroupingType,
                    GroupingOptionsSplitType = attributes.GroupingOptionsSplitType,
                    GroupingOptionsSplitSize = attributes.GroupingOptionsSplitSize,
                    GroupingOptionsSplitSeparator = attributes.GroupingOptionsSplitSeparator,
                    GroupingOptionsSplitPosition = attributes.GroupingOptionsSplitPosition,
                },
                Relationships = relationships,
            },
            idempotencyKey,
            cancellationToken
        );
    }
}

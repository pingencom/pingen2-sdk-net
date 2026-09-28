using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Batches.Statistics;
using Pingen.Abstractions.Models.Batches.Views;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen batches service endpoint. <see href="https://api.pingen.com/documentation#tag/batches.general">API Doc - Batches General</see>
/// </summary>
public interface IBatchService : IConnectorService
{
    /// <summary>
    /// Get a collection of batches. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.list">API Doc - Batches list</see>
    /// </summary>>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<BatchData>>> GetPage([Optional] ApiPagingRequest? apiPagingRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Create a new batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.create">API Doc - Batches create</see>
    /// <br/>Important: The 3-Step Process to Create a new batch
    /// <br/>1. Make a GET Request to the (File upload) endpoint to request an upload url. Use <see cref="IFilesService.GetPath"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>2. Send the raw PDF or ZIP Binary file via PUT Request (NOT Form-Post) to the url received in Step 1. Use <see cref="IFilesService.UploadFile"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>3. Make a POST Request to the Create Batch Endpoint passing the file url and file signature you received in Step 1. <see cref="Create"/>
    /// <br/>Set <c>channel_type</c> on <see cref="BatchCreate"/> to create a postal, ebill or email batch (any of <see cref="Abstractions.Enums.Batches.BatchChannelTypes"/>); it defaults to <c>post</c>.
    /// </summary>
    /// <param name="data">Data for POST</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchDataDetailed>>> Create(
        DataPost<BatchCreate, BatchCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get details of a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.show">API Doc - Get details of a batch</see>
    /// </summary>
    /// <param name="batchId">ID of the batch to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchDataDetailed>>> Get(
        string batchId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<BatchData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Send a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.send">API Doc - Batches send</see>
    /// <br/>The request's JSON:API <c>type</c> is channel-specific (<c>batches_channel_{post|email|ebill}_send</c>); build the payload with <see cref="Abstractions.Models.Batches.Views.BatchSendRequest"/> so the type and attributes match the batch's channel.
    /// </summary>
    /// <param name="data">Data for PATCH, best built via <see cref="Abstractions.Models.Batches.Views.BatchSendRequest"/>. Postal batches carry a delivery product plus print mode and spectrum; email/ebill batches carry only the delivery product.</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchDataDetailed>>> Send(
        DataPatch<BatchSend> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Cancel a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.cancel">API Doc - Batches cancel</see>
    /// </summary>
    /// <param name="batchId">ID of the batch to cancel</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Cancel(string batchId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Delete a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.delete">API Doc - Batches delete</see>
    /// </summary>
    /// <param name="batchId">ID of the batch to delete</param>
    /// <param name="withDeliverables">Optional, also delete the deliverables (letters) contained in the batch. Default false.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Delete(string batchId, [Optional] bool withDeliverables, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Edit a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/batches.edit">API Doc - Batches edit</see>
    /// </summary>
    /// <param name="data">Data for PATCH (name and/or icon)</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchDataDetailed>>> Update(
        DataPatch<BatchEdit> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get statistics of a batch. <see href="https://api.pingen.com/documentation#tag/batches.general/operation/organisations.batches.statistics.details">API Doc - Batch statistics</see>
    /// </summary>
    /// <param name="batchId">ID of the batch</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchStatisticsData>>> GetStatistics(
        string batchId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Add an attachment (a previously uploaded file) to a batch.
    /// <br/>NOTE: Not part of the currently published OpenAPI spec; supported on a best-effort basis.
    /// </summary>
    /// <param name="batchId">ID of the batch to attach the file to</param>
    /// <param name="data">Attachment data (file url + signature) for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> AddAttachment(
        string batchId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Convenience: upload a file and create a batch in one call (file-upload URL → PUT → create).
    /// </summary>
    /// <param name="file">The raw PDF/ZIP bytes to upload.</param>
    /// <param name="attributes">Batch attributes (without the file url/signature, which are filled automatically).</param>
    /// <param name="relationships">Optional, preset relationship.</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<BatchDataDetailed>>> UploadAndCreate(
        Stream file,
        BatchUpload attributes,
        [Optional] BatchCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );
}

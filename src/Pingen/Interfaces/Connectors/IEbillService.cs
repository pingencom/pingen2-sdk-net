using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Ebills.Views;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen ebill service endpoint. <see href="https://api.pingen.com/documentation#tag/deliveries.ebills">API Doc - Ebills</see>
/// </summary>
public interface IEbillService : IConnectorService
{
    /// <summary>
    /// Get a collection of ebills.
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<EbillData>>> GetPage([Optional] ApiPagingRequest? apiPagingRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Call <see cref="GetPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<EbillData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Create a new ebill delivery.
    /// <br/>Important: The 3-Step Process to Create a new ebill
    /// <br/>1. Make a GET Request to the (File upload) endpoint to request an upload url. Use <see cref="IFilesService.GetPath"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>2. Send the raw PDF Binary file via PUT Request (NOT Form-Post) to the url received in Step 1. Use <see cref="IFilesService.UploadFile"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>3. Make a POST Request to the Create Ebill Endpoint passing the file url and file signature you received in Step 1. <see cref="Create"/>
    /// </summary>
    /// <param name="data">Data for POST</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<EbillDataDetailed>>> Create(
        DataPost<EbillCreate, EbillCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get details of an ebill.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<EbillDataDetailed>>> Get(
        string ebillId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Send an ebill (submit it for delivery). Body-less PATCH.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to send</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Send(string ebillId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Cancel an ebill.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to cancel</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Cancel(string ebillId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Delete an ebill.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to delete</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Delete(string ebillId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Get file of ebill. (Redirect URL only)
    /// <br/>API returns a 302 Found with the file URL in Location header. Get url from <see cref="ApiResult.Location"/>.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to get the file from</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns><see cref="ApiResult"/> with <see cref="ApiResult.Location"/></returns>
    public Task<ApiResult> GetFileLocation(string ebillId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Download file from URL received by <see cref="GetFileLocation"/>
    /// </summary>
    /// <exception cref="PingenFileDownloadException"></exception>
    public Task<Stream> DownloadFileContent(Uri fileUrl, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Add an attachment (a previously uploaded file) to an ebill.
    /// <br/>NOTE: Not part of the currently published OpenAPI spec; supported on a best-effort basis.
    /// </summary>
    /// <param name="ebillId">ID of the ebill to attach the file to</param>
    /// <param name="data">Attachment data (file url + signature) for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> AddAttachment(
        string ebillId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Convenience: upload a file and create an ebill in one call (file-upload URL → PUT → create).
    /// </summary>
    /// <param name="file">The raw PDF bytes to upload.</param>
    /// <param name="attributes">Ebill attributes (without the file url/signature, which are filled automatically).</param>
    /// <param name="relationships">Optional, preset relationship.</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<EbillDataDetailed>>> UploadAndCreate(
        Stream file,
        EbillUpload attributes,
        [Optional] EbillCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );
}

using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.LetterPrices;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Letters.Views;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen letter service endpoint. <see href="https://api.pingen.com/documentation#tag/letters.general">API Doc - Letters General</see>
/// </summary>
public interface ILetterService : IConnectorService
{
    /// <summary>
    /// Get a collection of letters. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.list">API Doc - Letters list</see>
    /// </summary>>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterData>>> GetPage([Optional] ApiPagingRequest? apiPagingRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Call <see cref="GetPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Create a new letter. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.create">API Doc - Letters create</see>
    /// <br/>Important: The 3-Step Process to Create a new letter
    /// <br/>1. Make a GET Request to the (File upload) endpoint to request an upload url. Use <see cref="IFilesService.GetPath"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>2. Send the raw PDF Binary file via PUT Request (NOT Form-Post) to the url received in Step 1. Use <see cref="IFilesService.UploadFile"/> on <see cref="IPingenApiClient.Files"/>
    /// <br/>3. Make a POST Request to the Create Letter Endpoint passing the file url and file signature you received in Step 1. <see cref="Create"/>
    /// </summary>
    /// <param name="data">Data for POST</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<LetterDataDetailed>>> Create(
        DataPost<LetterCreate, LetterCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Send a letter. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.send">API Doc - Letters send</see>
    /// </summary>
    /// <param name="data">Data for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<LetterDataDetailed>>> Send(
        DataPatch<LetterSend> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Send a letter. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.send">API Doc - Letters send</see>
    /// </summary>
    /// <param name="letterId">ID of the letter to cancel</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Cancel(string letterId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Get details of a letter. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.show">API Doc - Letters show</see>
    /// </summary>
    /// <param name="letterId">ID of the letter to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<LetterDataDetailed>>> Get(
        string letterId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Delete a letter. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.delete">API Doc - Letters delete</see>
    /// </summary>
    /// <param name="letterId">ID of the letter to delete</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Delete(string letterId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Get file of letter. (Redirect URL only) <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.file">API Doc - Letters file</see>
    /// <br/>API returns a 302 Found with the file URL in Location header. Get url from <see cref="ApiResult.Location"/>.
    /// </summary>
    /// <param name="letterId">ID of the letter to get the file from</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns><see cref="ApiResult"/> with <see cref="ApiResult.Location"/></returns>
    public Task<ApiResult> GetFileLocation(string letterId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Download file from URL received by <see cref="GetFileLocation"/> <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.file">API Doc - Letters file</see>
    /// </summary>
    /// <exception cref="PingenFileDownloadException"></exception>
    public Task<Stream> DownloadFileContent(Uri fileUrl, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Calculate price for given letter configuration. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.price-calculator">API Doc - Letters price calculator</see>
    /// </summary>
    /// <param name="data">Data for POST</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<LetterPriceData>>> CalculatePrice(
        DataPost<LetterPriceConfiguration> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Add an attachment (a previously uploaded file) to a letter.
    /// <br/>NOTE: Not part of the currently published OpenAPI spec; supported on a best-effort basis.
    /// </summary>
    /// <param name="letterId">ID of the letter to attach the file to</param>
    /// <param name="data">Attachment data (file url + signature) for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> AddAttachment(
        string letterId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Add an attachment (a previously uploaded file) to multiple letters at once.
    /// <br/>NOTE: Not part of the currently published OpenAPI spec; supported on a best-effort basis.
    /// </summary>
    /// <param name="data">Attachment data (letter ids + file url + signature) for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> AddAttachmentToMultipleLetters(
        DataPost<AddAttachmentToMultipleLetters> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Convenience: upload a file and create a letter in one call (file-upload URL → PUT → create).
    /// </summary>
    /// <param name="file">The raw PDF bytes to upload.</param>
    /// <param name="attributes">Letter attributes (without the file url/signature, which are filled automatically).</param>
    /// <param name="relationships">Optional, preset relationship.</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<LetterDataDetailed>>> UploadAndCreate(
        Stream file,
        LetterUpload attributes,
        [Optional] LetterCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );
}

using System.Xml;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors.Base;

/// <inheritdoc />
public abstract class ConnectorService : IConnectorService
{
    /// <summary>
    /// Pingen connection handler
    /// </summary>
    protected readonly IPingenConnectionHandler ConnectionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectorService"/> class. Inject connection handler at construction.
    /// </summary>
    protected ConnectorService(IPingenConnectionHandler pingenConnectionHandler)
    {
        ConnectionHandler = pingenConnectionHandler;
    }

    /// <summary>
    /// Request a file-upload URL and PUT the given content to it (steps 1 and 2 of the 3-step upload flow),
    /// returning the file reference (url + signature) used to create a deliverable. Shared by the
    /// <c>UploadAndCreate</c> convenience methods.
    /// </summary>
    /// <param name="fileContent">The raw PDF/ZIP bytes to upload.</param>
    /// <returns>The uploaded file reference.</returns>
    /// <exception cref="PingenApiErrorException">The file-upload URL request failed.</exception>
    /// <exception cref="InvalidOperationException">The upload to storage failed.</exception>
    protected async Task<FileUploadData> RequestAndUploadFile(Stream fileContent, CancellationToken cancellationToken)
    {
        ApiResult<SingleResult<FileUploadData>> pathResult = await ConnectionHandler.GetAsync<SingleResult<FileUploadData>>(
            FileUploadEndpoints.FileUpload,
            apiPagingRequest: null,
            cancellationToken: cancellationToken
        );

        if (!pathResult.IsSuccess || pathResult.Data?.Data is null)
            throw new PingenApiErrorException(pathResult);

        FileUploadData fileUploadData = pathResult.Data.Data;

        using var request = new HttpRequestMessage(HttpMethod.Put, fileUploadData.Attributes.Url) { Content = new StreamContent(fileContent) };
        using HttpResponseMessage response = await ConnectionHandler.SendExternalRequestAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"File upload to storage failed with HTTP status {(int)response.StatusCode} ({response.ReasonPhrase}).");

        return fileUploadData;
    }

    /// <summary>
    /// Download a deliverable's file content from a storage URL (shared by the letter/email/ebill channels).
    /// Returns a caller-owned, seekable stream; the response and connection are released before returning.
    /// </summary>
    /// <param name="fileUrl">The (pre-signed) storage URL to download from.</param>
    /// <exception cref="PingenFileDownloadException">The download failed; the message carries the storage error code or the HTTP status.</exception>
    protected async Task<Stream> DownloadFileContentInternal(Uri fileUrl, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
        using HttpResponseMessage response = await ConnectionHandler.SendExternalRequestAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new PingenFileDownloadException(TryGetStorageErrorCode(errorContent) ?? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}".Trim());
        }

        var buffer = new MemoryStream();
        await response.Content.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        return buffer;
    }

    /// <summary>
    /// Best-effort extraction of the error code from an S3-style XML error body. Returns the <c>Code</c> value
    /// (or <see cref="string.Empty"/> when the XML parses but has no <c>Code</c> element), and <c>null</c> only
    /// when the body is empty or not XML (e.g. an HTML gateway page or a JSON error) - so a malformed error body
    /// falls back to the HTTP status instead of masking the download failure with an <see cref="XmlException"/>.
    /// </summary>
    private static string? TryGetStorageErrorCode(string errorContent)
    {
        if (string.IsNullOrWhiteSpace(errorContent))
            return null;

        try
        {
            var xml = new XmlDocument();
            xml.LoadXml(errorContent);

            return xml.SelectSingleNode("/Error/Code/text()")?.Value ?? string.Empty;
        }
        catch (XmlException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public IList<TData> HandleResult<TData>(ApiResult<CollectionResult<TData>> apiResult)
        where TData : IData
    {
        if (!apiResult.IsSuccess)
            throw new PingenApiErrorException(apiResult);

        return apiResult.Data is null ? [] : apiResult.Data.Data;
    }

    /// <inheritdoc />
    public TData HandleResult<TData>(ApiResult<SingleResult<TData>> apiResult)
        where TData : IData
    {
        if (!apiResult.IsSuccess || apiResult.Data is null)
            throw new PingenApiErrorException(apiResult);

        return apiResult.Data.Data;
    }

    /// <summary>
    /// Generic auto page async
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="getPage">Function to get page</param>
    /// <returns>Pages from result of getPage function asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    protected async IAsyncEnumerable<IEnumerable<TData>> AutoPage<TData>(
        ApiPagingRequest? apiPagingRequest,
        Func<ApiPagingRequest, Task<ApiResult<CollectionResult<TData>>>> getPage
    )
        where TData : IData
    {
        var apiReRequest = (apiPagingRequest ?? new ApiPagingRequest()) with { PageNumber = apiPagingRequest?.PageNumber ?? 1 };

        ApiResult<CollectionResult<TData>> result;
        do
        {
            result = await getPage.Invoke(apiReRequest);
            HandleResult(result);
            yield return result.Data?.Data ?? [];

            apiReRequest = apiReRequest with { PageNumber = apiReRequest.PageNumber + 1 };
        } while (result.Data?.Meta is not null && result.Data.Meta.CurrentPage < result.Data.Meta.LastPage);
    }
}

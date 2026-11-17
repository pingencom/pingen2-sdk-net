using System.Net;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IFilesService" />
public sealed class FilesService : ConnectorService, IFilesService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilesService"/> class.
    /// </summary>
    public FilesService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<FileUploadData>>> GetPath([Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync<SingleResult<FileUploadData>>(
            requestPath: FileUploadEndpoints.FileUpload,
            apiPagingRequest: null,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ExternalRequestResult> UploadFile(FileUploadData fileUploadData, Stream data, [Optional] CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, fileUploadData.Attributes.Url) { Content = new StreamContent(data) };

        using var response = await ConnectionHandler.SendExternalRequestAsync(request, cancellationToken);

        return new ExternalRequestResult
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            ReasonPhrase = response.ReasonPhrase,
        };
    }
}

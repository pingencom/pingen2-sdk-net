using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Ebills.Views;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IEbillService" />
public sealed class EbillService : ConnectorService, IEbillService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EbillService"/> class.
    /// </summary>
    public EbillService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<EbillData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<EbillData>>(EbillsEndpoints.Root, apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<EbillData>> GetPageResultsAsync(
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
    public async Task<ApiResult<SingleResult<EbillDataDetailed>>> Create(
        DataPost<EbillCreate, EbillCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "ebills" };

        return await ConnectionHandler.PostAsync<SingleResult<EbillDataDetailed>, DataPost<EbillCreate, EbillCreateRelationships>>(
            EbillsEndpoints.Root,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<EbillDataDetailed>>> Get(
        string ebillId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<EbillDataDetailed>>(
            requestPath: EbillsEndpoints.Single(ebillId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Send(string ebillId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.PatchAsync(EbillsEndpoints.Send(ebillId), idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> Cancel(string ebillId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.PatchAsync(EbillsEndpoints.Cancel(ebillId), idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> Delete(string ebillId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.DeleteAsync(EbillsEndpoints.Single(ebillId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetFileLocation(string ebillId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(requestPath: EbillsEndpoints.File(ebillId), apiPagingRequest: null, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadFileContent(Uri fileUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(fileUrl, cancellationToken);

    /// <inheritdoc />
    public async Task<ApiResult> AddAttachment(
        string ebillId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "ebills" };

        return await ConnectionHandler.PatchAsync(EbillsEndpoints.Attachment(ebillId), data, idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<EbillDataDetailed>>> UploadAndCreate(
        Stream file,
        EbillUpload attributes,
        [Optional] EbillCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        FileUploadData fileUploadData = await RequestAndUploadFile(file, cancellationToken);

        return await Create(
            new DataPost<EbillCreate, EbillCreateRelationships>
            {
                Type = "ebills",
                Attributes = new EbillCreate
                {
                    FileOriginalName = attributes.FileOriginalName,
                    FileUrl = fileUploadData.Attributes.Url,
                    FileUrlSignature = fileUploadData.Attributes.UrlSignature,
                    AutoSend = attributes.AutoSend,
                    MetaData = attributes.MetaData,
                },
                Relationships = relationships,
            },
            idempotencyKey,
            cancellationToken
        );
    }
}

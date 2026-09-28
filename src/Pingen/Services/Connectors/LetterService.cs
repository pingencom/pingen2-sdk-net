using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Files;
using Pingen.Abstractions.Models.LetterPrices;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Letters.Views;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.ILetterService" />
public sealed class LetterService : ConnectorService, ILetterService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LetterService"/> class.
    /// </summary>
    public LetterService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterData>>(LettersEndpoints.Root, apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterData>> GetPageResultsAsync(
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
    public async Task<ApiResult<SingleResult<LetterDataDetailed>>> Create(
        DataPost<LetterCreate, LetterCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "letters" };

        return await ConnectionHandler.PostAsync<SingleResult<LetterDataDetailed>, DataPost<LetterCreate, LetterCreateRelationships>>(
            LettersEndpoints.Root,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<LetterDataDetailed>>> Send(
        DataPatch<LetterSend> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "letters" };

        return await ConnectionHandler.PatchAsync<SingleResult<LetterDataDetailed>, DataPatch<LetterSend>>(
            LettersEndpoints.Send(data.Id),
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Cancel(string letterId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.PatchAsync(LettersEndpoints.Cancel(letterId), idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<LetterDataDetailed>>> Get(
        string letterId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<LetterDataDetailed>>(
            requestPath: LettersEndpoints.Single(letterId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Delete(string letterId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.DeleteAsync(LettersEndpoints.Single(letterId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetFileLocation(string letterId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(requestPath: LettersEndpoints.File(letterId), apiPagingRequest: null, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadFileContent(Uri fileUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(fileUrl, cancellationToken);

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<LetterPriceData>>> CalculatePrice(
        DataPost<LetterPriceConfiguration> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "letter_price_calculator" };

        return await ConnectionHandler.PostAsync<SingleResult<LetterPriceData>, DataPost<LetterPriceConfiguration>>(
            LettersEndpoints.PriceCalculator,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> AddAttachment(
        string letterId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "letters" };

        return await ConnectionHandler.PatchAsync(LettersEndpoints.Attachment(letterId), data, idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> AddAttachmentToMultipleLetters(
        DataPost<AddAttachmentToMultipleLetters> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "letters_attachment" };

        return await ConnectionHandler.PatchAsync(LettersEndpoints.AttachmentMultiple, data, idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<LetterDataDetailed>>> UploadAndCreate(
        Stream file,
        LetterUpload attributes,
        [Optional] LetterCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        FileUploadData fileUploadData = await RequestAndUploadFile(file, cancellationToken);

        return await Create(
            new DataPost<LetterCreate, LetterCreateRelationships>
            {
                Type = "letters",
                Attributes = new LetterCreate
                {
                    FileOriginalName = attributes.FileOriginalName,
                    FileUrl = fileUploadData.Attributes.Url,
                    FileUrlSignature = fileUploadData.Attributes.UrlSignature,
                    AddressPosition = attributes.AddressPosition,
                    AutoSend = attributes.AutoSend,
                    DeliveryProduct = attributes.DeliveryProduct,
                    PrintMode = attributes.PrintMode,
                    PrintSpectrum = attributes.PrintSpectrum,
                    MetaData = attributes.MetaData,
                },
                Relationships = relationships,
            },
            idempotencyKey,
            cancellationToken
        );
    }
}

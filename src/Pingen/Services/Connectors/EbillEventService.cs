using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.EbillEvents;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="IEbillEventService" />
public sealed class EbillEventService : ConnectorService, IEbillEventService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EbillEventService"/> class.
    /// </summary>
    public EbillEventService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<EbillEventData>>> GetEventsPage(
        string ebillId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<EbillEventData>>(
            EbillsEndpoints.Events(ebillId, language),
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<EbillEventData>> GetEventsPageResultsAsync(
        string ebillId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetEventsPage(ebillId, language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetEventImage(string ebillId, string eventId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(
            requestPath: EbillsEndpoints.EventImage(ebillId, eventId),
            apiPagingRequest: null,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadEventImage(Uri imageUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(imageUrl, cancellationToken);
}

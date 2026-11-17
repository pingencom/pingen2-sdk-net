using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.BatchEvents;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="IBatchEventService" />
public sealed class BatchEventService : ConnectorService, IBatchEventService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchEventService"/> class.
    /// </summary>
    public BatchEventService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<BatchEventData>>> GetEventsPage(
        string batchId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<BatchEventData>>(
            BatchesEndpoints.Events(batchId, language),
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<BatchEventData>> GetEventsPageResultsAsync(
        string batchId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetEventsPage(batchId, language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }
}

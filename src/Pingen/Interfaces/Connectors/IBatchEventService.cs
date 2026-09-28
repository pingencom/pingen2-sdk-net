using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.BatchEvents;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen batch events service. <see href="https://api.pingen.com/documentation#tag/batches.events">API Doc - Batch events</see>
/// </summary>
public interface IBatchEventService : IConnectorService
{
    /// <summary>
    /// Get a collection of events of a batch. <see href="https://api.pingen.com/documentation#tag/batches.events/operation/batches.events">API Doc - Batch events</see>
    /// </summary>
    /// <param name="batchId">ID of batch to get events</param>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<BatchEventData>>> GetEventsPage(
        string batchId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetEventsPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="batchId">ID of batch to get events</param>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetEventsPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<BatchEventData>> GetEventsPageResultsAsync(
        string batchId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );
}

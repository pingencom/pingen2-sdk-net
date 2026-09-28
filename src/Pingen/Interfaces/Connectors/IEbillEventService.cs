using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.EbillEvents;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen ebill events service. <see href="https://api.pingen.com/documentation#tag/ebills.events">API Doc - Ebill events</see>
/// </summary>
public interface IEbillEventService : IConnectorService
{
    /// <summary>
    /// Get a collection of events of an ebill. <see href="https://api.pingen.com/documentation#tag/ebills.events/operation/ebills.events">API Doc - Ebill events</see>
    /// </summary>
    /// <param name="ebillId">ID of ebill to get events</param>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<EbillEventData>>> GetEventsPage(
        string ebillId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetEventsPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="ebillId">ID of ebill to get events</param>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetEventsPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<EbillEventData>> GetEventsPageResultsAsync(
        string ebillId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get the image of an ebill event. The API returns a 302 with the file URL in the Location header; read it
    /// from <see cref="ApiResult.Location"/> and pass it to <see cref="DownloadEventImage"/>.
    /// </summary>
    /// <param name="ebillId">ID of the ebill</param>
    /// <param name="eventId">ID of the ebill event</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns><see cref="ApiResult"/> with <see cref="ApiResult.Location"/></returns>
    public Task<ApiResult> GetEventImage(string ebillId, string eventId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Download the event image from the URL received via <see cref="GetEventImage"/>.
    /// </summary>
    /// <param name="imageUrl">The image URL from <see cref="ApiResult.Location"/>.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<Stream> DownloadEventImage(Uri imageUrl, [Optional] CancellationToken cancellationToken);
}

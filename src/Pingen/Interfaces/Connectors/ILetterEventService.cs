using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen letter events service. <see href="https://api.pingen.com/documentation#tag/letters.events">API Doc - Letters events</see>
/// </summary>
public interface ILetterEventService : IConnectorService
{
    /// <summary>
    /// Get a collection of events of a single letter. <see href="https://api.pingen.com/documentation#tag/letters.events/operation/letters.events">API Doc - Letter events</see>
    /// </summary>
    /// <param name="letterId">ID of letter to get events</param>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterEventData>>> GetEventsPage(
        string letterId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetEventsPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="letterId">ID of letter to get events</param>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetEventsPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterEventData>> GetEventsPageResultsAsync(
        string letterId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get a collection of "issues" events across all letters. <see href="https://api.pingen.com/documentation#tag/letters.events/operation/letters.issues">API Doc - Letters issues</see>
    /// </summary>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterEventData>>> GetIssuesPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetIssuesPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetIssuesPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterEventData>> GetIssuesPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get a collection of "sent" events across all letters. <see href="https://api.pingen.com/documentation#tag/letters.events/operation/organisations.letters.events.sent">API Doc - Letters events sent</see>
    /// </summary>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterEventData>>> GetSentPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetSentPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetSentPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterEventData>> GetSentPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get a collection of "undeliverable" events across all letters. <see href="https://api.pingen.com/documentation#tag/letters.events/operation/organisations.letters.events.undeliverable">API Doc - Letters events undeliverable</see>
    /// </summary>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterEventData>>> GetUndeliverablePage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetUndeliverablePage"/> and auto page until end of collection
    /// </summary>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetUndeliverablePage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterEventData>> GetUndeliverablePageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get a collection of "delivered" events across all letters. <see href="https://api.pingen.com/documentation#tag/letters.events/operation/organisations.letters.events.delivered">API Doc - Letters events delivered</see>
    /// </summary>
    /// <param name="language">Language for events. Culture names required, e.g. 'en-GB'. Use any of <see cref="PingenApiLanguage"/></param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<LetterEventData>>> GetDeliveredPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetDeliveredPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="language">Language for events</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>Pages from <see cref="GetDeliveredPage"/> asynchronously</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<LetterEventData>> GetDeliveredPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get the image of a letter event. The API returns a 302 with the file URL in the Location header; read it
    /// from <see cref="ApiResult.Location"/> and pass it to <see cref="DownloadEventImage"/>.
    /// </summary>
    /// <param name="letterId">ID of the letter</param>
    /// <param name="eventId">ID of the letter event</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns><see cref="ApiResult"/> with <see cref="ApiResult.Location"/></returns>
    public Task<ApiResult> GetEventImage(string letterId, string eventId, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Download the event image from the URL received via <see cref="GetEventImage"/>.
    /// </summary>
    /// <param name="imageUrl">The image URL from <see cref="ApiResult.Location"/>.</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<Stream> DownloadEventImage(Uri imageUrl, [Optional] CancellationToken cancellationToken);
}

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="ILetterEventService" />
public sealed class LetterEventService : ConnectorService, ILetterEventService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LetterEventService"/> class.
    /// </summary>
    public LetterEventService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterEventData>>> GetEventsPage(
        string letterId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterEventData>>(
            LettersEndpoints.Events(letterId, language),
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterEventData>> GetEventsPageResultsAsync(
        string letterId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetEventsPage(letterId, language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterEventData>>> GetIssuesPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterEventData>>(LettersEndpoints.Issues(language), apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterEventData>> GetIssuesPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetIssuesPage(language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterEventData>>> GetSentPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterEventData>>(LettersEndpoints.Sent(language), apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterEventData>> GetSentPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetSentPage(language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterEventData>>> GetUndeliverablePage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterEventData>>(
            LettersEndpoints.Undeliverable(language),
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterEventData>> GetUndeliverablePageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetUndeliverablePage(language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<LetterEventData>>> GetDeliveredPage(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<LetterEventData>>(LettersEndpoints.Delivered(language), apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<LetterEventData>> GetDeliveredPageResultsAsync(
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetDeliveredPage(language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetEventImage(string letterId, string eventId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(
            requestPath: LettersEndpoints.EventImage(letterId, eventId),
            apiPagingRequest: null,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadEventImage(Uri imageUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(imageUrl, cancellationToken);
}

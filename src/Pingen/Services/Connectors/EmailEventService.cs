using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.EmailEvents;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="IEmailEventService" />
public sealed class EmailEventService : ConnectorService, IEmailEventService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailEventService"/> class.
    /// </summary>
    public EmailEventService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<EmailEventData>>> GetEventsPage(
        string emailId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<EmailEventData>>(
            EmailsEndpoints.Events(emailId, language),
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<EmailEventData>> GetEventsPageResultsAsync(
        string emailId,
        string language,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetEventsPage(emailId, language, apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetEventImage(string emailId, string eventId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(
            requestPath: EmailsEndpoints.EventImage(emailId, eventId),
            apiPagingRequest: null,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadEventImage(Uri imageUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(imageUrl, cancellationToken);
}

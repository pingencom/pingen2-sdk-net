using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.Views;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IWebhookService" />
public sealed class WebhookService : ConnectorService, IWebhookService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookService"/> class.
    /// </summary>
    public WebhookService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<WebhookData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<WebhookData>>(WebhooksEndpoints.Root, apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<WebhookData>> GetPageResultsAsync(
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
    public async Task<ApiResult<SingleResult<WebhookData>>> Create(
        DataPost<WebhookCreate> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "webhooks" };

        return await ConnectionHandler.PostAsync<SingleResult<WebhookData>, DataPost<WebhookCreate>>(
            WebhooksEndpoints.Root,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<WebhookData>>> Get(
        string webhookId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<WebhookData>>(
            requestPath: WebhooksEndpoints.Single(webhookId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Delete(string webhookId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.DeleteAsync(WebhooksEndpoints.Single(webhookId), cancellationToken);
    }
}

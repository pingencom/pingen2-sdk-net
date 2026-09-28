using System.Runtime.InteropServices;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.Views;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen webhook service endpoint. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks">API Doc - Webhooks</see>
/// </summary>
public interface IWebhookService
{
    /// <summary>
    /// Get a collection of webhooks. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks/operation/webhooks.index">API Doc - Webhooks list</see>
    /// </summary>>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<WebhookData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<WebhookData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Create new webhook. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks/operation/webhooks">API Doc - Webhooks create</see>
    /// </summary>
    /// <param name="data">Data for POST</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<WebhookData>>> Create(
        DataPost<WebhookCreate> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get details of a webhook. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks/operation/webhooks.show">API Doc - Webhooks show</see>
    /// </summary>
    /// <param name="webhookId">ID of the webhook to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<WebhookData>>> Get(string webhookId, [Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Delete a webhook. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks/operation/webhooks.destroy">API Doc - Webhooks delete</see>
    /// </summary>
    /// <param name="webhookId">ID of the webhook to delete</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> Delete(string webhookId, [Optional] CancellationToken cancellationToken);
}

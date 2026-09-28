using System.Runtime.InteropServices;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api;

namespace Pingen.Interfaces;

/// <summary>
/// Connection handler to call Pingen REST API
/// </summary>
public interface IPingenConnectionHandler
{
    /// <summary>
    /// Change the organisation ID to use for upcoming requests
    /// </summary>
    /// <param name="organisationId">Id to use for all requests at /organisations/{organisationId}/*</param>
    public void SetOrganisationId(string organisationId);

    /// <summary>
    /// Base GET request
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<TResult>> GetAsync<TResult>(
        string requestPath,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult;

    /// <summary>
    /// Base GET request with base API request (e.g. for sparse fieldsets on single-resource endpoints)
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<TResult>> GetAsync<TResult>(string requestPath, [Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken)
        where TResult : IDataResult;

    /// <summary>
    /// Base GET request
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> GetAsync(string requestPath, [Optional] ApiPagingRequest? apiPagingRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Base GET request with base API request (e.g. for sparse fieldsets on single-resource endpoints)
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> GetAsync(string requestPath, [Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Base POST request with payload
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="dataPost">Data POST object to send to the API</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<TResult>> PostAsync<TResult, TPost>(
        string requestPath,
        TPost dataPost,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
        where TPost : IDataPost;

    /// <summary>
    /// Base DELETE request
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> DeleteAsync(string requestPath, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Base DELETE request with a JSON:API body (required by some endpoints, e.g. batch delete)
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="data">Body data (identifier + attributes) for DELETE</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> DeleteAsync<TPost>(string requestPath, TPost data, [Optional] CancellationToken cancellationToken)
        where TPost : IDataPost;

    /// <summary>
    /// Base PATCH request without payload
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> PatchAsync(string requestPath, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Base PATCH request with payload
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="data">Data for PATCH</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. To be able to safely retry these kind of API calls, you can set the HTTP Header Idempotency-Key with any unique 1-64 character string. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<TResult>> PatchAsync<TResult, TPatch>(
        string requestPath,
        TPatch data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
        where TPatch : IDataPatch;

    /// <summary>
    /// Base PATCH request with a data-post payload but no returned data (e.g. attachment endpoints).
    /// </summary>
    /// <param name="requestPath">Relative request path</param>
    /// <param name="data">Data POST object to send to the API</param>
    /// <param name="idempotencyKey">Optional, unique request identifier for idempotency. <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see></param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult> PatchAsync<TPost>(string requestPath, TPost data, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
        where TPost : IDataPost;

    /// <summary>
    /// Send an external request. This is not an API call, but a request to an absolute URL without authentication or other pre-configured headers.
    /// </summary>
    public Task<HttpResponseMessage> SendExternalRequestAsync(HttpRequestMessage request, [Optional] CancellationToken cancellationToken);
}

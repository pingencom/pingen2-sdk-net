using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Web;
using Pingen.Abstractions.Constants;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Interfaces;
using Pingen.Models;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services;

/// <inheritdoc />
public sealed class PingenConnectionHandler : IPingenConnectionHandler
{
    /// <summary>
    /// Endpoints that do not use organisation ID in path
    /// </summary>
    private static readonly string[] NonOrganisationEndpoints = [FileUploadEndpoints.FileUpload, UsersEndpoints.Root, OrganisationsEndpoints.Root];

    /// <summary>
    /// Pingen configuration for connection handler
    /// </summary>
    private readonly IPingenConfiguration _configuration;

    /// <summary>
    /// Pingen http clients
    /// </summary>
    private readonly PingenHttpClients _httpClients;

    /// <summary>
    /// The organisation ID to use for all request at /organisations/{organisationId}/*
    /// </summary>
    private string? _organisationId;

    /// <summary>
    /// Process-wide access-token cache. Holds no per-request state, so it is safe to share across scopes (as a
    /// singleton) while the per-request organisation id above stays on this scoped handler.
    /// </summary>
    private readonly PingenAccessTokenProvider _accessTokenProvider;

    /// <summary>
    /// Cache key isolating this handler's credentials (identity endpoint + client id) in the shared provider.
    /// </summary>
    private readonly string _tokenCacheKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenConnectionHandler"/> class with its own private
    /// token cache (for standalone, non-DI usage).
    /// </summary>
    public PingenConnectionHandler(IPingenConfiguration configuration, PingenHttpClients httpClients)
        : this(configuration, httpClients, new PingenAccessTokenProvider()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenConnectionHandler"/> class sharing the given token
    /// cache. Used by DI so one <see cref="PingenAccessTokenProvider"/> singleton is shared across request scopes.
    /// </summary>
    internal PingenConnectionHandler(IPingenConfiguration configuration, PingenHttpClients httpClients, PingenAccessTokenProvider accessTokenProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration.BaseUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration.IdentityUri);

        _configuration = configuration.Normalize();
        _organisationId = _configuration.DefaultOrganisationId;
        _httpClients = httpClients;
        _accessTokenProvider = accessTokenProvider;
        _tokenCacheKey = $"{_configuration.IdentityUri}|{_configuration.ClientId}";
    }

    /// <summary>
    /// Authenticate against the identity endpoint and return the parsed access token (or throw on an auth
    /// error). The caching and refresh policy live in <see cref="PingenAccessTokenProvider"/>; this method only
    /// performs the HTTP exchange.
    /// </summary>
    /// <exception cref="PingenApiAuthErrorException">The token endpoint returned a non-success status.</exception>
    private async Task<AccessToken?> LoginAsync()
    {
        using var response = await _httpClients.Identity.PostAsync(
            "auth/access-tokens",
            new FormUrlEncodedContent([
                new("grant_type", "client_credentials"),
                new("client_id", _configuration.ClientId),
                new("client_secret", _configuration.ClientSecret),
            ])
        );

        if (!response.IsSuccessStatusCode)
        {
            var authError = await PingenSerialisationHelper.DeserializeAsync<PingenApiAuthError>(await response.Content.ReadAsStreamAsync());

            throw new PingenApiAuthErrorException(
                authError,
                authError?.Message ?? authError?.ErrorDescription ?? authError?.Error ?? "Failed to obtain access token."
            );
        }

        return await PingenSerialisationHelper.DeserializeAsync<AccessToken>(await response.Content.ReadAsStreamAsync());
    }

    /// <inheritdoc />
    public void SetOrganisationId(string organisationId)
    {
        _organisationId = organisationId;
    }

    /// <inheritdoc />
    public async Task<ApiResult<TResult>> GetAsync<TResult>(
        string requestPath,
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
    {
        return await GetApiResult<TResult>(
            await SendWithRetryAsync(() => GetHttpRequestMessage(HttpMethod.Get, requestPath, apiPagingRequest), retryableOn5xx: true, cancellationToken)
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<TResult>> GetAsync<TResult>(
        string requestPath,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
    {
        return await GetApiResult<TResult>(
            await SendWithRetryAsync(() => GetHttpRequestMessage(HttpMethod.Get, requestPath, apiRequest), retryableOn5xx: true, cancellationToken)
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetAsync(string requestPath, [Optional] ApiPagingRequest? apiPagingRequest, [Optional] CancellationToken cancellationToken)
    {
        return await GetApiResult(
            await SendWithRetryAsync(() => GetHttpRequestMessage(HttpMethod.Get, requestPath, apiPagingRequest), retryableOn5xx: true, cancellationToken)
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetAsync(string requestPath, [Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken)
    {
        return await GetApiResult(
            await SendWithRetryAsync(() => GetHttpRequestMessage(HttpMethod.Get, requestPath, apiRequest), retryableOn5xx: true, cancellationToken)
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<TResult>> PostAsync<TResult, TPost>(
        string requestPath,
        TPost dataPost,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
        where TPost : IDataPost
    {
        return await GetApiResult<TResult>(
            await SendWithRetryAsync(
                () => GetHttpRequestMessageWithBody(HttpMethod.Post, requestPath, dataPost, null, idempotencyKey),
                retryableOn5xx: !string.IsNullOrEmpty(idempotencyKey),
                cancellationToken
            )
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> DeleteAsync(string requestPath, [Optional] CancellationToken cancellationToken)
    {
        return await GetApiResult(
            await SendWithRetryAsync(
                () => WithJsonApiContentType(GetHttpRequestMessage(HttpMethod.Delete, requestPath)),
                retryableOn5xx: true,
                cancellationToken
            )
        );
    }

    /// <inheritdoc cref="DeleteAsync{TPost}" />
    public async Task<ApiResult> DeleteAsync<TPost>(string requestPath, TPost data, [Optional] CancellationToken cancellationToken)
        where TPost : IDataPost
    {
        return await GetApiResult(
            await SendWithRetryAsync(() => GetHttpRequestMessageWithBody(HttpMethod.Delete, requestPath, data), retryableOn5xx: true, cancellationToken)
        );
    }

    /// <inheritdoc cref="PatchAsync" />
    public async Task<ApiResult> PatchAsync(string requestPath, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await GetApiResult(
            await SendWithRetryAsync(
                () => WithJsonApiContentType(GetHttpRequestMessage(HttpMethod.Patch, requestPath, null, idempotencyKey)),
                retryableOn5xx: !string.IsNullOrEmpty(idempotencyKey),
                cancellationToken
            )
        );
    }

    /// <summary>
    /// Attach the JSON:API content type to a bodyless mutating request (DELETE / bodyless PATCH). The Pingen API
    /// requires the <c>application/vnd.api+json</c> content type even when there is no request body.
    /// </summary>
    private static HttpRequestMessage WithJsonApiContentType(HttpRequestMessage request)
    {
        request.Content = new StringContent(string.Empty, new MediaTypeHeaderValue("application/vnd.api+json"));
        return request;
    }

    /// <inheritdoc cref="PatchAsync{TResult,TPost}" />
    public async Task<ApiResult<TResult>> PatchAsync<TResult, TPatch>(
        string requestPath,
        TPatch data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
        where TResult : IDataResult
        where TPatch : IDataPatch
    {
        return await GetApiResult<TResult>(
            await SendWithRetryAsync(
                () => GetHttpRequestMessageWithBody(HttpMethod.Patch, requestPath, data, null, idempotencyKey),
                retryableOn5xx: !string.IsNullOrEmpty(idempotencyKey),
                cancellationToken
            )
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> PatchAsync<TPost>(
        string requestPath,
        TPost data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
        where TPost : IDataPost
    {
        return await GetApiResult(
            await SendWithRetryAsync(
                () => GetHttpRequestMessageWithBody(HttpMethod.Patch, requestPath, data, null, idempotencyKey),
                retryableOn5xx: !string.IsNullOrEmpty(idempotencyKey),
                cancellationToken
            )
        );
    }

    /// <summary>
    /// Base exponential-backoff unit in milliseconds (doubled each attempt).
    /// </summary>
    private const int BaseBackoffMs = 250;

    /// <summary>
    /// Upper bound (milliseconds) for a <c>Retry-After</c>-derived delay, so a hostile header cannot stall a request indefinitely.
    /// </summary>
    private const int MaxRetryAfterMs = 10_000;

    /// <summary>
    /// Upper bound (milliseconds) for the exponential-backoff delay, so a high <see cref="IPingenConfiguration.MaxRetryAttempts"/>
    /// cannot produce multi-hour waits.
    /// </summary>
    private const int MaxBackoffMs = 30_000;

    /// <summary>
    /// Send a request with automatic retry and a one-time reactive token refresh.
    /// <list type="bullet">
    ///   <item>On <c>401</c> the access token is force-refreshed once and the request is retried; this retry
    ///   does not consume the retry budget.</item>
    ///   <item><c>429</c> is always retried; <c>5xx</c> is retried when <paramref name="retryableOn5xx"/> is
    ///   <c>true</c> (idempotent GET/PUT/DELETE always, POST/PATCH only when an idempotency key is set).</item>
    ///   <item>Between attempts an exponential backoff with ±20% jitter is applied, honouring
    ///   <c>Retry-After</c> (capped at <see cref="MaxRetryAfterMs"/>).</item>
    /// </list>
    /// The request is rebuilt from <paramref name="requestFactory"/> on every attempt because an
    /// <see cref="HttpRequestMessage"/> can only be sent once.
    /// </summary>
    /// <param name="requestFactory">Builds a fresh request message for each attempt.</param>
    /// <param name="retryableOn5xx">Whether <c>5xx</c> responses are safe to retry for this request.</param>
    private async Task<HttpResponseMessage> SendWithRetryAsync(
        Func<HttpRequestMessage> requestFactory,
        bool retryableOn5xx,
        CancellationToken cancellationToken
    )
    {
        var token = await _accessTokenProvider.GetValidTokenAsync(_tokenCacheKey, LoginAsync);

        var maxAttempts = Math.Max(1, _configuration.MaxRetryAttempts);
        var refreshedOn401 = false;

        for (var attempt = 0; ; attempt++)
        {
            // A fresh request per attempt (an HttpRequestMessage can only be sent once); disposed each iteration.
            // The bearer token is set per request rather than on the shared HttpClient, so a concurrent refresh
            // never mutates a header another in-flight request is reading.
            using var request = requestFactory();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClients.Api.SendAsync(request, cancellationToken);

            // One-time reactive token refresh on 401 - does not consume the retry budget.
            if (response.StatusCode == HttpStatusCode.Unauthorized && !refreshedOn401)
            {
                refreshedOn401 = true;
                response.Dispose();
                token = await _accessTokenProvider.ForceRefreshAsync(_tokenCacheKey, token, LoginAsync);
                attempt--;
                continue;
            }

            if (attempt < maxAttempts - 1 && IsRetryableStatus(response, retryableOn5xx))
            {
                var delay = ComputeBackoff(response, attempt);
                response.Dispose();
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            return response;
        }
    }

    /// <summary>
    /// Whether a response status should be retried.
    /// </summary>
    private static bool IsRetryableStatus(HttpResponseMessage response, bool retryableOn5xx)
    {
        var code = (int)response.StatusCode;
        if (code == (int)HttpStatusCode.TooManyRequests)
            return true;

        return retryableOn5xx && code is >= 500 and <= 599;
    }

    /// <summary>
    /// Compute the backoff delay before the next attempt: <c>max(Retry-After, BaseBackoffMs · 2^attempt)</c>
    /// multiplied by a ±20% jitter.
    /// </summary>
    private static TimeSpan ComputeBackoff(HttpResponseMessage response, int attempt)
    {
        var retryAfterMs = 0;
        if (response.Headers.RetryAfter?.Delta is { } delta)
            retryAfterMs = (int)Math.Min(delta.TotalMilliseconds, MaxRetryAfterMs);

        var exponentialMs = Math.Min(BaseBackoffMs * (1 << Math.Min(attempt, 16)), MaxBackoffMs);
        var baseMs = Math.Max(retryAfterMs, exponentialMs);
        var jitter = 0.8 + (Random.Shared.NextDouble() * 0.4);

        return TimeSpan.FromMilliseconds(baseMs * jitter);
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> SendExternalRequestAsync(HttpRequestMessage request, [Optional] CancellationToken cancellationToken) =>
        await _httpClients.External.SendAsync(request, cancellationToken);

    /// <summary>
    /// Get http request message to send to the API (with body as json from data with type T)
    /// </summary>
    private HttpRequestMessage GetHttpRequestMessageWithBody<T>(
        HttpMethod httpMethod,
        string requestPath,
        T? payload,
        [Optional] ApiRequest? apiRequest,
        [Optional] string? idempotencyKey
    )
        where T : IDataPost
    {
        var httpRequestMessage = GetHttpRequestMessage(httpMethod, requestPath, apiRequest, idempotencyKey);

        httpRequestMessage.Content = payload is null
            ? null
            : new StringContent(PingenSerialisationHelper.Serialize(new { data = payload }), new MediaTypeHeaderValue("application/vnd.api+json"));

        return httpRequestMessage;
    }

    /// <summary>
    /// Get http request message to send to the API (without body)
    /// </summary>
    private HttpRequestMessage GetHttpRequestMessage(
        HttpMethod httpMethod,
        string requestPath,
        [Optional] ApiRequest? apiRequest,
        [Optional] string? idempotencyKey
    )
    {
        if (string.IsNullOrWhiteSpace(_organisationId))
            throw new InvalidOperationException("Organisation ID has not been set");

        var httpRequestMessage = new HttpRequestMessage { Method = httpMethod };

        var uriBuilder = new UriBuilder(
            new Uri(
                _httpClients.Api.BaseAddress!,
                Array.Exists(NonOrganisationEndpoints, requestPath.StartsWith) ? requestPath : $"{OrganisationsEndpoints.Single(_organisationId)}/{requestPath}"
            )
        );

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var (key, value) in GetRequestHeaders(idempotencyKey))
            httpRequestMessage.Headers.Add(key, value);

        foreach (var (key, value) in GetQueryParameters(apiRequest))
            query[key] = value;

        uriBuilder.Query = query.ToString();
        httpRequestMessage.RequestUri = uriBuilder.Uri;

        return httpRequestMessage;
    }

    /// <summary>
    /// Get API request headers
    /// </summary>
    private static IEnumerable<KeyValuePair<string, string>> GetRequestHeaders([Optional] string? idempotencyKey)
    {
        if (!string.IsNullOrEmpty(idempotencyKey))
            yield return new(ApiHeaderNames.IdempotencyKey, idempotencyKey);
    }

    /// <summary>
    /// Get API request query parameters
    /// </summary>
    private static IEnumerable<KeyValuePair<string, string>> GetQueryParameters(ApiRequest? apiRequest)
    {
        if (apiRequest is null)
            yield break;

        if (apiRequest.SparseFieldsets?.Any() is true)
            foreach (var (dataType, fields) in apiRequest.SparseFieldsets)
                yield return new(ApiQueryParameterNames.SparseFields(dataType), string.Join(',', fields));

        if (apiRequest.Include?.Any() is true)
            yield return new(ApiQueryParameterNames.Include, string.Join(',', apiRequest.Include));

        if (apiRequest is not ApiPagingRequest apiPagingRequest)
            yield break;

        foreach (var keyValuePair in GetQueryParameters(apiPagingRequest))
            yield return keyValuePair;
    }

    /// <summary>
    /// Get API request query parameters
    /// </summary>
    private static IEnumerable<KeyValuePair<string, string>> GetQueryParameters(ApiPagingRequest apiPagingRequest)
    {
        if (apiPagingRequest.Sorting?.Any() is true)
            yield return new(
                ApiQueryParameterNames.Sorting,
                string.Join(
                    ',',
                    apiPagingRequest.Sorting.Select(entry =>
                        $"{(entry.Value is CollectionSortDirection.DESC ? "-" : string.Empty)}{entry.Key[..1].ToLower() + entry.Key[1..]}"
                    )
                )
            );

        if (apiPagingRequest.Filtering.HasValue)
            yield return new(ApiQueryParameterNames.Filtering, PingenSerialisationHelper.Serialize(apiPagingRequest.Filtering.Value));

        if (!string.IsNullOrEmpty(apiPagingRequest.Searching))
            yield return new(ApiQueryParameterNames.Searching, apiPagingRequest.Searching);

        if (apiPagingRequest.PageNumber.HasValue)
            yield return new(ApiQueryParameterNames.PageNumber, apiPagingRequest.PageNumber.Value.ToString());

        if (apiPagingRequest.PageLimit.HasValue)
            yield return new(ApiQueryParameterNames.PageLimit, apiPagingRequest.PageLimit.Value.ToString());
    }

    /// <summary>
    /// Get API result from response
    /// </summary>
    private static async Task<ApiResult<T>> GetApiResult<T>(HttpResponseMessage httpResponseMessage)
        where T : IDataResult
    {
        using (httpResponseMessage)
        {
            var isSuccess = httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode is HttpStatusCode.Found;
            var headers = GetResponseHeaders(httpResponseMessage);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            return new()
            {
                IsSuccess = isSuccess,
                RequestId = (Guid)(headers[ApiHeaderNames.RequestId] ?? Guid.Empty),
                RateLimitLimit = (int)(headers[ApiHeaderNames.RateLimitLimit] ?? 0),
                RateLimitRemaining = (int)(headers[ApiHeaderNames.RateLimitRemaining] ?? 0),
                RateLimitReset = (DateTimeOffset?)headers[ApiHeaderNames.RateLimitReset],
                RetryAfter = (int?)headers[ApiHeaderNames.RetryAfter],
                IdempotentReplayed = (bool)(headers[ApiHeaderNames.IdempotentReplayed] ?? false),
                ApiError = isSuccess ? null : PingenSerialisationHelper.Deserialize<ApiError>(content),
                Location = (Uri?)headers[ApiHeaderNames.Location],
                Data = httpResponseMessage.IsSuccessStatusCode ? PingenSerialisationHelper.Deserialize<T>(content) : default,
            };
        }
    }

    /// <summary>
    /// Get API result from response
    /// </summary>
    private static async Task<ApiResult> GetApiResult(HttpResponseMessage httpResponseMessage)
    {
        using (httpResponseMessage)
        {
            var isSuccess = httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode is HttpStatusCode.Found;
            var headers = GetResponseHeaders(httpResponseMessage);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            return new()
            {
                IsSuccess = isSuccess,
                RequestId = (Guid)(headers[ApiHeaderNames.RequestId] ?? Guid.Empty),
                RateLimitLimit = (int)(headers[ApiHeaderNames.RateLimitLimit] ?? 0),
                RateLimitRemaining = (int)(headers[ApiHeaderNames.RateLimitRemaining] ?? 0),
                RateLimitReset = (DateTimeOffset?)headers[ApiHeaderNames.RateLimitReset],
                RetryAfter = (int?)headers[ApiHeaderNames.RetryAfter],
                IdempotentReplayed = (bool)(headers[ApiHeaderNames.IdempotentReplayed] ?? false),
                ApiError = isSuccess ? null : PingenSerialisationHelper.Deserialize<ApiError>(content),
                Location = (Uri?)headers[ApiHeaderNames.Location],
            };
        }
    }

    /// <summary>
    /// Get API headers from response
    /// </summary>
    private static Dictionary<string, object?> GetResponseHeaders(HttpResponseMessage httpResponseMessage)
    {
        return new()
        {
            [ApiHeaderNames.RequestId] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.RequestId, out var values) && Guid.TryParse(values.First(), out var requestId)
                    ? requestId
                    : Guid.Empty,

            [ApiHeaderNames.RateLimitLimit] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.RateLimitLimit, out values) && int.TryParse(values.First(), out var rateLimitLimit)
                    ? rateLimitLimit
                    : 0,

            [ApiHeaderNames.RateLimitRemaining] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.RateLimitRemaining, out values)
                && int.TryParse(values.First(), out var rateLimitRemaining)
                    ? rateLimitRemaining
                    : 0,

            [ApiHeaderNames.RateLimitReset] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.RateLimitReset, out values) && long.TryParse(values.First(), out var rateLimitResetUnix)
                    ? DateTimeOffset.FromUnixTimeSeconds(rateLimitResetUnix)
                    : null,

            [ApiHeaderNames.RetryAfter] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.RetryAfter, out values) && int.TryParse(values.First(), out var retryAfter)
                    ? retryAfter
                    : null,

            [ApiHeaderNames.IdempotentReplayed] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.IdempotentReplayed, out values)
                && bool.TryParse(values.First(), out var idempotentReplayed)
                && idempotentReplayed,

            [ApiHeaderNames.Location] =
                httpResponseMessage.Headers.TryGetValues(ApiHeaderNames.Location, out values)
                && Uri.TryCreate(values.First(), UriKind.Absolute, out var location)
                    ? location
                    : null,
        };
    }
}

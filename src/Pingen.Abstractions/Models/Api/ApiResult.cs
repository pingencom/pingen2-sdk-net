using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Api;

/// <summary>
/// An API result with meta information received from the Pingen API
/// </summary>
public record ApiResult
{
    /// <summary>
    /// Indicates if the API call was successfully or not
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// UUID. Can be used for support inquiries.
    /// <see href="https://api.pingen.com/documentation#section/Basics/HTTP-methods-and-headers">API Doc - HTTP</see>
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// The number of allowed requests in the current period.
    /// <see href="https://api.pingen.com/documentation#section/Basics/HTTP-methods-and-headers">API Doc - HTTP</see>
    /// </summary>
    public int RateLimitLimit { get; init; }

    /// <summary>
    /// The number of remaining requests in the current period.
    /// <see href="https://api.pingen.com/documentation#section/Basics/HTTP-methods-and-headers">API Doc - HTTP</see>
    /// </summary>
    public int RateLimitRemaining { get; init; }

    /// <summary>
    /// The timestamp when the current period expires and the request can be retried. (in UTC)
    /// <see href="https://api.pingen.com/documentation#section/Basics/Throttling-Rate-limiting">API Doc - Throttling</see>
    /// </summary>
    public DateTimeOffset? RateLimitReset { get; init; }

    /// <summary>
    /// The number of seconds you have to wait until you can retry (and the current period expires).
    /// <see href="https://api.pingen.com/documentation#section/Basics/Throttling-Rate-limiting">API Doc - Throttling</see>
    /// </summary>
    public int? RetryAfter { get; init; }

    /// <summary>
    /// You can determine if an idempotent API call was replayed by inspecting the Idempotent-Replayed header for the value true.
    /// <see href="https://api.pingen.com/documentation#section/Advanced/Idempotency">API Doc - Idempotency</see>
    /// </summary>
    public bool IdempotentReplayed { get; init; }

    /// <summary>
    /// API errors, set when failed. <see cref="ApiResult{T}.Data"/> might be empty in that case.
    /// </summary>
    public ApiError? ApiError { get; init; }

    /// <summary>
    /// Location Url, for result 302 Found
    /// </summary>
    public Uri? Location { get; init; }
}

/// <summary>
/// An API result with meta information and data received from the Pingen API
/// </summary>
public sealed record ApiResult<T> : ApiResult
    where T : IDataResult
{
    /// <summary>
    /// Data received from the API. Check <see cref="ApiResult.IsSuccess"/> and <see cref="ApiResult.ApiError"/>, especially when Data is null.
    /// </summary>
    public T? Data { get; init; }
}

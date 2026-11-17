namespace Pingen.Abstractions.Constants;

/// <summary>
/// API header names for requests or from responses
/// </summary>
public static class ApiHeaderNames
{
    #region HeadersToSend

    /// <summary>
    /// Header name for Idempotency key
    /// </summary>
    public const string IdempotencyKey = "Idempotency-Key";

    #endregion HeadersToSends

    #region HeadersToReceive

    /// <summary>
    /// Header name for request ID
    /// </summary>
    public const string RequestId = "X-Request-ID"; // API-DOC: X-Request-ID

    /// <summary>
    /// Header name for rate limit limit
    /// </summary>
    public const string RateLimitLimit = "x-ratelimit-limit"; // API-DOC: X-Rate-Limit-Limit

    /// <summary>
    /// Header name for rate limit remaining
    /// </summary>
    public const string RateLimitRemaining = "x-ratelimit-remaining"; // API-DOC: X-Rate-Limit-Remaining

    /// <summary>
    /// Header name for rate limit reset
    /// </summary>
    public const string RateLimitReset = "x-ratelimit-reset"; // API-DOC: X-Rate-Limit-Reset

    /// <summary>
    /// Header name for retry after
    /// </summary>
    public const string RetryAfter = "Retry-After";

    /// <summary>
    /// Header name for idempotent replayed
    /// </summary>
    public const string IdempotentReplayed = "Idempotent-Replayed";

    /// <summary>
    /// Header name for location
    /// </summary>
    public const string Location = "Location";

    #endregion HeadersToReceive
}

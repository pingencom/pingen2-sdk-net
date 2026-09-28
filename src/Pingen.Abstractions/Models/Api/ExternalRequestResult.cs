using System.Net;

namespace Pingen.Abstractions.Models.Api;

/// <summary>
/// Result of an external HTTP request (e.g., S3 file upload). Unlike <see cref="ApiResult"/>,
/// this does not carry Pingen API-specific metadata such as request IDs or rate limit headers.
/// </summary>
public sealed record ExternalRequestResult
{
    /// <summary>
    /// Indicates whether the HTTP request completed successfully (2xx status code)
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The HTTP status code returned by the external server
    /// </summary>
    public HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// The reason phrase returned by the external server (e.g., "OK", "Forbidden", "Not Found").
    /// May be null if the server did not include a reason phrase.
    /// </summary>
    public string? ReasonPhrase { get; init; }
}

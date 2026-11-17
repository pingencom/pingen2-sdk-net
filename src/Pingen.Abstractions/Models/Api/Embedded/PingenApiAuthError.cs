using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// Error returned by the OAuth token endpoint when acquiring an access token fails. Its shape (an
/// <c>error</c> / <c>error_description</c> pair) differs from the JSON:API <see cref="ApiError"/> used for
/// resource requests. Carried by <see cref="Exceptions.PingenApiAuthErrorException"/>.
/// </summary>
/// <param name="Error">Machine-readable OAuth error code, e.g. <c>invalid_client</c>.</param>
/// <param name="ErrorDescription">Human-readable description of the error.</param>
/// <param name="Message">Additional message returned by Pingen, when present.</param>
public sealed record PingenApiAuthError(
    [property: JsonPropertyName("error")] string? Error,
    [property: JsonPropertyName("error_description")] string? ErrorDescription,
    [property: JsonPropertyName("message")] string? Message
);

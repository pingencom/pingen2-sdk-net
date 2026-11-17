using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// API error data object
/// </summary>
/// <param name="Code">An application-specific error code, expressed as a string value</param>
/// <param name="Title">A short, human-readable summary of the problem</param>
/// <param name="Detail">Default: "null". A human-readable explanation specific to this occurrence of the problem. Like title, this field’s value can be localized</param>
/// <param name="Source">An object containing references to the source of the error</param>
public sealed record ApiErrorData(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("detail")] string Detail,
    [property: JsonPropertyName("source")] ApiErrorSource Source
);

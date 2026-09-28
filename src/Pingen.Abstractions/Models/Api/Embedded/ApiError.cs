using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// API error
/// </summary>
public record ApiError([property: JsonPropertyName("errors")] IList<ApiErrorData> Errors);

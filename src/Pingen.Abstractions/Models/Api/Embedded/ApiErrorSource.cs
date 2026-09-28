using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// API error source
/// </summary>
/// <param name="Pointer">Default: "null". A JSON Pointer [RFC6901] to the associated entity in the request document [e.g. '/data' for a primary data object, or '/data/attributes/title' for a specific attribute]</param>
/// <param name="Parameter">Default: "null". A string indicating which URI query parameter caused the error</param>
public sealed record ApiErrorSource([property: JsonPropertyName("pointer")] string Pointer, [property: JsonPropertyName("parameter")] string Parameter);

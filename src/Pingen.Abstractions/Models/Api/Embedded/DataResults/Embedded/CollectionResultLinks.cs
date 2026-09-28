using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.DataResults.Embedded;

/// <summary>
/// List links
/// </summary>
public sealed record CollectionResultLinks(
    [property: JsonPropertyName("first")] string First,
    [property: JsonPropertyName("last")] string Last,
    [property: JsonPropertyName("prev")] string Prev,
    [property: JsonPropertyName("next")] string Next,
    [property: JsonPropertyName("self")] string Self
);

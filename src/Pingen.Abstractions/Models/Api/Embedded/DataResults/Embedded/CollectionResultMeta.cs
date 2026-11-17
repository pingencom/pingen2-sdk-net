using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.DataResults.Embedded;

/// <summary>
/// List meta
/// </summary>
public sealed record CollectionResultMeta(
    [property: JsonPropertyName("current_page")] int? CurrentPage,
    [property: JsonPropertyName("last_page")] int? LastPage,
    [property: JsonPropertyName("per_page")] int? PerPage,
    [property: JsonPropertyName("from")] int? From,
    [property: JsonPropertyName("to")] int? To,
    [property: JsonPropertyName("total")] int? Total
);

using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Batches.Statistics.Embedded;

/// <summary>
/// A grouping of letters within a batch (e.g. by split group).
/// </summary>
public sealed record BatchLetterGroup([property: JsonPropertyName("name")] string? Name, [property: JsonPropertyName("count")] int? Count);

using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Batches.Statistics.Embedded;

/// <summary>
/// A per-country (or per-region) letter count within a batch.
/// </summary>
public sealed record BatchLetterCountry([property: JsonPropertyName("country")] string? Country, [property: JsonPropertyName("count")] int? Count);

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Batches.Statistics.Embedded;

namespace Pingen.Abstractions.Models.Batches.Statistics;

/// <summary>
/// Batch statistics (JSON:API type <c>batch_details_statistics</c>).
/// </summary>
/// <param name="LetterValidating">Number of letters still validating.</param>
/// <param name="LetterGroups">Letters grouped by split group.</param>
/// <param name="LetterCountries">Letters grouped by destination country.</param>
/// <param name="LetterRegions">Letters grouped by destination region.</param>
[PingenApiType("batch_details_statistics")]
public sealed record BatchStatistics(
    [property: JsonPropertyName("letter_validating")] int? LetterValidating,
    [property: JsonPropertyName("letter_groups")] IReadOnlyList<BatchLetterGroup>? LetterGroups,
    [property: JsonPropertyName("letter_countries")] IReadOnlyList<BatchLetterCountry>? LetterCountries,
    [property: JsonPropertyName("letter_regions")] IReadOnlyList<BatchLetterCountry>? LetterRegions
) : IAttributes;

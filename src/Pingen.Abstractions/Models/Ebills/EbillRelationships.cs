using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Ebill relationships
/// </summary>
public sealed record EbillRelationships(
    [property: JsonPropertyName("organisation")] RelatedSingleOutput Organisation,
    [property: JsonPropertyName("events")] RelatedManyOutput Events,
    [property: JsonPropertyName("batch")] RelatedSingleOutput Batch
) : IRelationships;

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Batch relationships
/// </summary>
public sealed record BatchRelationships(
    [property: JsonPropertyName("organisation")] RelatedSingleOutput Organisation,
    [property: JsonPropertyName("events")] RelatedManyOutput Events
) : IRelationships;

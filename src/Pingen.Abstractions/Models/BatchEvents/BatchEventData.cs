using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;
using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.BatchEvents;

/// <summary>
/// Batch event data object
/// </summary>
public sealed record BatchEventData : Data<BatchEvent, BatchEventRelationships>;

/// <summary>
/// Batch event relationships
/// </summary>
/// <param name="Batch">The batch this event belongs to.</param>
public sealed record BatchEventRelationships([property: JsonPropertyName("batch")] RelatedSingleOutput Batch) : IRelationships;

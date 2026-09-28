using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.BatchEvents;

/// <summary>
/// Batch event
/// </summary>
[PingenApiType("batches_events")]
public sealed record BatchEvent(
    [property: JsonPropertyName(BatchEventFields.Code)] string? Code,
    [property: JsonPropertyName(BatchEventFields.Name)] string? Name,
    [property: JsonPropertyName(BatchEventFields.Producer)] string? Producer,
    [property: JsonPropertyName(BatchEventFields.Location)] string? Location,
    [property: JsonPropertyName(BatchEventFields.Data)] IReadOnlyList<string>? Data,
    [property: JsonPropertyName(BatchEventFields.EmittedAt)] DateTime? EmittedAt,
    [property: JsonPropertyName(BatchEventFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(BatchEventFields.UpdatedAt)] DateTime? UpdatedAt
) : IAttributes;

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.LetterEvents;

/// <summary>
/// Letter event
/// </summary>
/// <param name="Code">Machine-readable event code.</param>
[PingenApiType("letters_events", "deliverables_events")]
public sealed record LetterEvent(
    [property: JsonPropertyName(LetterEventFields.Code)] string? Code,
    [property: JsonPropertyName(LetterEventFields.Name)] string? Name,
    [property: JsonPropertyName(LetterEventFields.Producer)] string? Producer,
    [property: JsonPropertyName(LetterEventFields.Location)] string? Location,
    [property: JsonPropertyName(LetterEventFields.HasImage)] bool? HasImage,
    [property: JsonPropertyName(LetterEventFields.Data)] IReadOnlyList<string>? Data,
    [property: JsonPropertyName(LetterEventFields.EmittedAt)] DateTime? EmittedAt,
    [property: JsonPropertyName(LetterEventFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(LetterEventFields.UpdatedAt)] DateTime? UpdatedAt
) : IAttributes;

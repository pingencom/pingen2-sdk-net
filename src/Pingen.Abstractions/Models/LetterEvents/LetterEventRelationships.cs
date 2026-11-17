using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.LetterEvents;

/// <summary>
/// Letter event relationships. Both <see cref="Letter"/> and its generalised counterpart
/// <see cref="Deliverable"/> are modelled and nullable, because the endpoint may return either shape - prefer
/// <see cref="Deliverable"/> and fall back to <see cref="Letter"/>.
/// </summary>
/// <param name="Letter">The letter this event belongs to; null on the deliverable-shaped payload.</param>
/// <param name="Deliverable">
/// The deliverable (<c>letter</c>, <c>email</c> or <c>ebill</c>) this event belongs to; null on the
/// letter-shaped payload.
/// </param>
public sealed record LetterEventRelationships(
    [property: JsonPropertyName("letter")] RelatedSingleOutput? Letter,
    [property: JsonPropertyName("deliverable")] RelatedSingleOutput? Deliverable = null
) : IRelationships;

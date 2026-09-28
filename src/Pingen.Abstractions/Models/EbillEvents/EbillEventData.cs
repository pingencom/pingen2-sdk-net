using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;

namespace Pingen.Abstractions.Models.EbillEvents;

/// <summary>
/// Ebill event data object. Ebill events share the deliverable-event attribute surface with letter events
/// (the spec's <c>DeliverableEventLuceneAttributes</c>), so they reuse the <see cref="LetterEvent"/>
/// attributes model. Their JSON:API <c>type</c> is <c>deliverables_events</c>.
/// </summary>
public sealed record EbillEventData : Data<LetterEvent, EbillEventRelationships>;

/// <summary>
/// Ebill event relationships
/// </summary>
/// <param name="Ebill">The ebill this event belongs to.</param>
/// <param name="Deliverable">The deliverable this event belongs to.</param>
public sealed record EbillEventRelationships(
    [property: JsonPropertyName("ebill")] RelatedSingleOutput? Ebill,
    [property: JsonPropertyName("deliverable")] RelatedSingleOutput? Deliverable = null
) : IRelationships;

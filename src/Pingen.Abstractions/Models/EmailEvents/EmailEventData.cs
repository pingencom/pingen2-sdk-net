using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;

namespace Pingen.Abstractions.Models.EmailEvents;

/// <summary>
/// Email event data object. Email events share the deliverable-event attribute surface with letter events
/// (the spec's <c>DeliverableEventLuceneAttributes</c>), so they reuse the <see cref="LetterEvent"/>
/// attributes model. Their JSON:API <c>type</c> is <c>deliverables_events</c>.
/// </summary>
public sealed record EmailEventData : Data<LetterEvent, EmailEventRelationships>;

/// <summary>
/// Email event relationships
/// </summary>
/// <param name="Email">The email this event belongs to.</param>
/// <param name="Deliverable">The deliverable this event belongs to.</param>
public sealed record EmailEventRelationships(
    [property: JsonPropertyName("email")] RelatedSingleOutput? Email,
    [property: JsonPropertyName("deliverable")] RelatedSingleOutput? Deliverable = null
) : IRelationships;

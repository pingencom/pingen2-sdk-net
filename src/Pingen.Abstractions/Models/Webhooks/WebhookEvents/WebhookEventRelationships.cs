using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Webhooks.WebhookEvents;

/// <summary>
/// Webhook event relationships. The documented shape is <c>organisation</c> + <c>deliverable</c> +
/// <c>event</c>; the legacy <c>letter</c> is no longer declared but is still sent on the wire. Both
/// <see cref="Letter"/> and <see cref="Deliverable"/> are modelled and nullable - prefer
/// <see cref="Deliverable"/> and fall back to <see cref="Letter"/>.
/// </summary>
/// <param name="Organisation">The organisation the deliverable belongs to.</param>
/// <param name="Letter">Legacy relationship to the letter (undocumented but still sent). Use <paramref name="Deliverable"/> instead.</param>
/// <param name="Event">The delivery event (<c>data.type</c> <c>deliverables_events</c>).</param>
/// <param name="Deliverable">The deliverable (<c>letter</c>, <c>email</c> or <c>ebill</c>) the event belongs to.</param>
public sealed record WebhookEventRelationships(
    [property: JsonPropertyName("organisation")] RelatedSingleOutput Organisation,
    [property: JsonPropertyName("letter")] RelatedSingleOutput? Letter,
    [property: JsonPropertyName("event")] RelatedSingleOutput Event,
    [property: JsonPropertyName("deliverable")] RelatedSingleOutput? Deliverable = null
) : IRelationships;

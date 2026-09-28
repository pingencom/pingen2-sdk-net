using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents.Embedded;

namespace Pingen.Abstractions.Models.Webhooks.WebhookEvents;

/// <summary>
/// Webhook event, shared by all four event categories (<c>issues</c>, <c>sent</c>, <c>undeliverable</c>,
/// <c>delivered</c>). Properties not sent for a given category are null.
/// </summary>
/// <param name="Reason">Why the item failed. Sent on <c>issues</c> and <c>undeliverable</c> only.</param>
/// <param name="CorrectedAddress">
/// The address Pingen worked out for an undeliverable item. Sent on <c>undeliverable</c> only, where the
/// spec marks it required; null for every other category.
/// </param>
[PingenApiType("webhook_issues", "webhook_sent", "webhook_undeliverable", "webhook_delivered")]
public sealed record WebhookEvent(
    [property: JsonPropertyName(WebhookEventFields.Reason)] string? Reason,
    [property: JsonPropertyName(WebhookEventFields.Url)] Uri? Url,
    [property: JsonPropertyName(WebhookEventFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(WebhookEventFields.CorrectedAddress)] WebhookEventCorrectedAddress? CorrectedAddress = null
) : IAttributes;

using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.Webhooks.WebhookEvents;

/// <summary>
/// Webhook event data object
/// </summary>
public sealed record WebhookEventData : Data<WebhookEvent, WebhookEventRelationships>;

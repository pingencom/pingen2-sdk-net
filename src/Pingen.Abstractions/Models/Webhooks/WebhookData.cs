using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.Webhooks;

/// <summary>
/// Webhook data object
/// </summary>
public sealed record WebhookData : Data<Webhook, WebhookRelationships>;

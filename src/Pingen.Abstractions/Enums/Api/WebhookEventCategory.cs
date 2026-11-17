namespace Pingen.Abstractions.Enums.Api;

/// <summary>
/// Categories a webhook can subscribe to (value of a webhook's <c>event_category</c>). Bound as strings, not a
/// C# enum, so a category Pingen adds later (as it did with <see cref="ChannelSubscriptions"/>) still
/// deserializes instead of breaking the whole webhook response.
/// <br/>Incoming <see cref="ChannelSubscriptions"/> event bodies are not modelled and are not processed by
/// <c>PingenWebhookHelper</c>.
/// </summary>
public static class WebhookEventCategory
{
    /// <summary>
    /// Issues on sent documents.
    /// </summary>
    public const string Issues = "issues";

    /// <summary>
    /// Undeliverable documents.
    /// </summary>
    public const string Undeliverable = "undeliverable";

    /// <summary>
    /// Sent documents.
    /// </summary>
    public const string Sent = "sent";

    /// <summary>
    /// Delivered sent documents: letters posted with delivery confirmation, e.g. registered mail.
    /// </summary>
    public const string Delivered = "delivered";

    /// <summary>
    /// Channel subscriptions.
    /// </summary>
    public const string ChannelSubscriptions = "channel_subscriptions";
}

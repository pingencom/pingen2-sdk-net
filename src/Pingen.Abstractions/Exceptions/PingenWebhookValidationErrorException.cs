using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Abstractions.Exceptions;

/// <summary>
///
/// </summary>
public class PingenWebhookValidationErrorException : PingenException
{
    /// <summary>
    /// API Result
    /// </summary>
    public WebhookEventData? WebhookEventData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenWebhookValidationErrorException"/> class
    /// </summary>
    public PingenWebhookValidationErrorException(WebhookEventData? webhookEventData)
    {
        WebhookEventData = webhookEventData;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenWebhookValidationErrorException"/> class
    /// </summary>
    public PingenWebhookValidationErrorException(WebhookEventData? webhookEventData, string message)
        : base(message)
    {
        WebhookEventData = webhookEventData;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenWebhookValidationErrorException"/> class
    /// </summary>
    public PingenWebhookValidationErrorException(WebhookEventData? webhookEventData, string message, Exception inner)
        : base(message, inner)
    {
        WebhookEventData = webhookEventData;
    }
}

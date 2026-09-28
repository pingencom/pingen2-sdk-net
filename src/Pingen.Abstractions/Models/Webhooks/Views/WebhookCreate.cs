using System.Text;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Webhooks.Views;

/// <summary>
/// Webhook create object to send via <see cref="DataPost{TAttributes}"/> to the API
/// </summary>
public sealed record WebhookCreate : IAttributes
{
    /// <summary>
    /// Redacts the signing key from the record's <see cref="object.ToString"/> output so it cannot leak into logs.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"EventCategory = {EventCategory}, ");
        builder.Append($"Url = {Url}, ");
        builder.Append("SigningKey = ***");

        return true;
    }

    /// <summary>
    /// Webhook event category - any of <see cref="WebhookEventCategory"/>.
    /// </summary>
    [JsonPropertyName("event_category")]
    public required string EventCategory { get; init; }

    /// <summary>
    /// Webhook URL
    /// </summary>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Webhook signing key
    /// </summary>
    [JsonPropertyName("signing_key")]
    public required string SigningKey { get; init; }
}

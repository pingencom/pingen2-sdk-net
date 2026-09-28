using System.Text;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Webhooks;

/// <summary>
/// Webhook
/// </summary>
[PingenApiType("webhooks")]
public sealed record Webhook(
    [property: JsonPropertyName(WebhookFields.EventCategory)] string? EventCategory,
    [property: JsonPropertyName(WebhookFields.Url)] Uri? Url,
    [property: JsonPropertyName(WebhookFields.SigningKey)] string? SigningKey
) : IAttributes
{
    /// <summary>
    /// Redacts the HMAC signing key from the record's <see cref="object.ToString"/> output so it cannot leak
    /// into logs.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"EventCategory = {EventCategory}, ");
        builder.Append($"Url = {Url}, ");
        builder.Append("SigningKey = ***");

        return true;
    }
}

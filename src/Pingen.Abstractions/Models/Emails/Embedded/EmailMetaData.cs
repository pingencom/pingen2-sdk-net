using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Emails.Views;

namespace Pingen.Abstractions.Models.Emails.Embedded;

/// <summary>
/// Meta data for <see cref="EmailCreate"/>. Carries the email routing information (sender, recipient,
/// subject and body) used when the deliverable is sent through the email channel.
/// </summary>
public sealed record EmailMetaData
{
    /// <summary>
    /// Display name of the sender.
    /// </summary>
    [JsonPropertyName("sender_name")]
    public required string SenderName { get; init; }

    /// <summary>
    /// Recipient email address.
    /// </summary>
    [JsonPropertyName("recipient_email")]
    public required string RecipientEmail { get; init; }

    /// <summary>
    /// Recipient display name.
    /// </summary>
    [JsonPropertyName("recipient_name")]
    public required string RecipientName { get; init; }

    /// <summary>
    /// Reply-to email address.
    /// </summary>
    [JsonPropertyName("reply_email")]
    public required string ReplyEmail { get; init; }

    /// <summary>
    /// Reply-to display name.
    /// </summary>
    [JsonPropertyName("reply_name")]
    public required string ReplyName { get; init; }

    /// <summary>
    /// Email subject.
    /// </summary>
    [JsonPropertyName("subject")]
    public required string Subject { get; init; }

    /// <summary>
    /// Email body content.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}

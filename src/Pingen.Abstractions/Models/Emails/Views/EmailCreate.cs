using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Emails.Embedded;

namespace Pingen.Abstractions.Models.Emails.Views;

/// <summary>
/// Email create object to send via <see cref="DataPost{TAttributes}"/> to the API
/// </summary>
public sealed record EmailCreate : IAttributes
{
    /// <summary>
    /// Filename [ 5 .. 255 ] characters
    /// </summary>
    [JsonPropertyName("file_original_name")]
    public required string FileOriginalName { get; init; }

    /// <summary>
    /// File URL [ 1 .. 1000 ] characters
    /// </summary>
    [JsonPropertyName("file_url")]
    public required string FileUrl { get; init; }

    /// <summary>
    /// File URL signature [ 1 .. 60 ] characters
    /// </summary>
    [JsonPropertyName("file_url_signature")]
    public required string FileUrlSignature { get; init; }

    /// <summary>
    /// Auto send
    /// </summary>
    [JsonPropertyName("auto_send")]
    public required bool AutoSend { get; init; }

    /// <summary>
    /// Meta data (sender, recipient, subject, body). Required to actually deliver the email.
    /// </summary>
    [JsonPropertyName("meta_data")]
    public EmailMetaData? MetaData { get; init; }
}

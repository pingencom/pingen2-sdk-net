using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Attachments;

/// <summary>
/// Attach a previously uploaded file to multiple letters at once, sent via <see cref="DataPost{TAttributes}"/>
/// (JSON:API type <c>letters_attachment</c>) to the <c>deliveries/letters/attachment</c> endpoint.
/// <para>
/// NOTE: This endpoint is not part of the currently published OpenAPI specification, so it is modelled on a
/// best-effort basis; verify against the live API before relying on it.
/// </para>
/// </summary>
public sealed record AddAttachmentToMultipleLetters : IAttributes
{
    /// <summary>
    /// IDs of the letters to attach the file to.
    /// </summary>
    [JsonPropertyName("letter_ids")]
    public required List<string> LetterIds { get; init; }

    /// <summary>
    /// File URL [ 1 .. 1000 ] characters (from the file-upload endpoint).
    /// </summary>
    [JsonPropertyName("file_url")]
    public required string FileUrl { get; init; }

    /// <summary>
    /// File URL signature [ 1 .. 60 ] characters (from the file-upload endpoint).
    /// </summary>
    [JsonPropertyName("file_url_signature")]
    public required string FileUrlSignature { get; init; }
}

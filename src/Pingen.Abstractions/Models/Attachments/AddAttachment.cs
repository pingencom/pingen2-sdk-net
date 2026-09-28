using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Attachments;

/// <summary>
/// Attach a previously uploaded file to a single deliverable (letter, email, ebill) or batch, sent via
/// <see cref="DataPost{TAttributes}"/> to the <c>.../{id}/attachment</c> endpoint.
/// <para>
/// NOTE: These attachment endpoints are not part of the currently published OpenAPI specification; modelled on
/// a best-effort basis, verify against the live API before relying on them.
/// </para>
/// </summary>
public sealed record AddAttachment : IAttributes
{
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

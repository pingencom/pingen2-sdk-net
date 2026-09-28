using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Files;

/// <summary>
/// File upload
/// </summary>
[PingenApiType("file_uploads")]
public sealed record FileUpload(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("url_signature")] string UrlSignature,
    [property: JsonPropertyName("expires_at")] DateTime? ExpiresAt
) : IAttributes;

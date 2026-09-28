using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Letters.Embedded;

/// <summary>
/// Letter font
/// </summary>
public sealed record LetterFont([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("is_embedded")] bool? IsEmbedded);

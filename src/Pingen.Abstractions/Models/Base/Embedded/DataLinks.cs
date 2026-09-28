using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Base.Embedded;

/// <summary>
/// Data links
/// </summary>
public sealed record DataLinks([property: JsonPropertyName("self")] string Self);

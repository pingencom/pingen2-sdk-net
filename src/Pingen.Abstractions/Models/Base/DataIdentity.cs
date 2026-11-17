using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Base;

/// <summary>
/// Base data identity - identifies an object by type and id.
/// </summary>
public record DataIdentity
{
    /// <summary>
    /// ID of the object.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// JSON:API resource type of the object (e.g. <c>letters</c>).
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}

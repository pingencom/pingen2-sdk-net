using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Batch-create relationships
/// </summary>
public record BatchCreateRelationships : IRelationships
{
    /// <summary>
    /// Preset relationship
    /// </summary>
    [JsonPropertyName("preset")]
    public required RelatedSingleInput Preset { get; init; }

    /// <summary>
    /// Create a new instance of <see cref="BatchCreateRelationships"/> for the given preset id.
    /// </summary>
    public static BatchCreateRelationships Create(string presetId) =>
        new()
        {
            Preset = new()
            {
                Data = new() { Id = presetId, Type = "presets" },
            },
        };
}

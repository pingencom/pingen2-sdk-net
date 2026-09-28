using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Emails.Views;

/// <summary>
/// Email-create relationships
/// </summary>
public record EmailCreateRelationships : IRelationships
{
    /// <summary>
    /// Preset relationship
    /// </summary>
    [JsonPropertyName("preset")]
    public required RelatedSingleInput Preset { get; init; }

    /// <summary>
    /// Create a new instance of <see cref="EmailCreateRelationships"/> for the given preset id.
    /// </summary>
    public static EmailCreateRelationships Create(string presetId) =>
        new()
        {
            Preset = new()
            {
                Data = new() { Id = presetId, Type = "presets" },
            },
        };
}

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Letters;

/// <summary>
/// Letter data object
/// </summary>
public record LetterData : Data<Letter, LetterRelationships>;

/// <summary>
/// Letter data object detailed
/// </summary>
public sealed record LetterDataDetailed : LetterData, IData<Letter, LetterRelationships, Meta<MetaAbility<LetterAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<LetterAbilities>> Meta { get; init; }
}

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Ebill data object
/// </summary>
public record EbillData : Data<Ebill, EbillRelationships>;

/// <summary>
/// Ebill data object detailed
/// </summary>
public sealed record EbillDataDetailed : EbillData, IData<Ebill, EbillRelationships, Meta<MetaAbility<EbillAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<EbillAbilities>> Meta { get; init; }
}

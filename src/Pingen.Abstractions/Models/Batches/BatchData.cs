using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Batch data object
/// </summary>
public record BatchData : Data<Batch, BatchRelationships>;

/// <summary>
/// Batch data object detailed
/// </summary>
public sealed record BatchDataDetailed : BatchData, IData<Batch, BatchRelationships, Meta<MetaAbility<BatchAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<BatchAbilities>> Meta { get; init; }
}

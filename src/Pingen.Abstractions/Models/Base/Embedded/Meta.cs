using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Base.Embedded;

/// <summary>
/// Meta object with abilities
/// </summary>
public record Meta<TMetaAbility>([property: JsonPropertyName("abilities")] TMetaAbility Abilities) : IMeta
    where TMetaAbility : IMetaAbility;

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Base.Embedded;

/// <summary>
/// Meta object with self abilities
/// </summary>
public record MetaAbility<TSelfAbilities>([property: JsonPropertyName("self")] TSelfAbilities Self) : IMetaAbility
    where TSelfAbilities : IAbilities;

/// <summary>
/// Meta object with self abilities and organisation abilities
/// </summary>
public record MetaAbilityWithOrganisation<TSelfAbilities, TOrganisationAbilities>(
    TSelfAbilities Self,
    [property: JsonPropertyName("organisation")] TOrganisationAbilities Organisation
) : MetaAbility<TSelfAbilities>(Self)
    where TSelfAbilities : IAbilities
    where TOrganisationAbilities : IAbilities;

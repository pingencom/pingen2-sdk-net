using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// User association data object
/// </summary>
public record UserAssociationData : Data<UserAssociation, UserAssociationRelationships>;

/// <summary>
/// User association data object detailed
/// </summary>
public sealed record UserAssociationDataDetailed
    : UserAssociationData,
        IData<UserAssociation, UserAssociationRelationships, Meta<MetaAbilityWithOrganisation<UserAssociationAbilities, OrganisationAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbilityWithOrganisation<UserAssociationAbilities, OrganisationAbilities>> Meta { get; init; }
}

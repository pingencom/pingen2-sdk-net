using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;
using Pingen.Abstractions.Models.UserAssociations;

namespace Pingen.Abstractions.Models.Organisations;

/// <summary>
/// Organisation data object
/// </summary>
public record OrganisationData : Data<Organisation, OrganisationRelationships>;

/// <summary>
/// Organisation data object detailed
/// </summary>
public sealed record OrganisationDataDetailed : OrganisationData, IData<Organisation, OrganisationRelationships, Meta<MetaAbility<OrganisationAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<OrganisationAbilities>> Meta { get; init; }
}

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// User association
/// </summary>
[PingenApiType("associations")]
public sealed record UserAssociation(
    [property: JsonPropertyName(UserAssociationFields.Role)] string? Role,
    [property: JsonPropertyName(UserAssociationFields.Status)] string? Status,
    [property: JsonPropertyName(UserAssociationFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(UserAssociationFields.UpdatedAt)] DateTime? UpdatedAt
) : IAttributes;

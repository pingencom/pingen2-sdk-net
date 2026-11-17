using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// User association relationships
/// </summary>
public sealed record UserAssociationRelationships([property: JsonPropertyName("organisation")] RelatedSingleOutput Organisation) : IRelationships;

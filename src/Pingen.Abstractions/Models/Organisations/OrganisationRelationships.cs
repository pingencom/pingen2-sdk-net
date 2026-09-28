using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Organisations;

/// <summary>
/// Organisation relationships
/// </summary>
public sealed record OrganisationRelationships([property: JsonPropertyName("associations")] RelatedManyOutput Associations) : IRelationships;

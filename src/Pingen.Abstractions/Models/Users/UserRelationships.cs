using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Users;

/// <summary>
/// User relationships
/// </summary>
public sealed record UserRelationships(
    [property: JsonPropertyName("associations")] RelatedManyOutput Associations,
    [property: JsonPropertyName("notifications")] RelatedManyOutput Notifications
) : IRelationships;

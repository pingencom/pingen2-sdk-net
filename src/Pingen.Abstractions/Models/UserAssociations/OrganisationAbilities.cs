using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// Organisation abilities
/// </summary>
public sealed record OrganisationAbilities([property: JsonPropertyName("manage")] PingenApiAbility Manage) : IAbilities;

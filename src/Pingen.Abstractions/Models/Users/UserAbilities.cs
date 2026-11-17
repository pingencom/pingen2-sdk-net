using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Users;

/// <summary>
/// User abilities
/// </summary>
public sealed record UserAbilities(
    [property: JsonPropertyName("reach")] PingenApiAbility Reach,
    [property: JsonPropertyName("act")] PingenApiAbility Act,
    [property: JsonPropertyName("resend-activation")] PingenApiAbility ResendActivation
) : IAbilities;

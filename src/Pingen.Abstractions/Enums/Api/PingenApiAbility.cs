using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Api;

/// <summary>
/// Pingen API ability
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PingenApiAbility>))]
public enum PingenApiAbility
{
    /// <summary>
    /// Ability OK
    /// </summary>
    ok,

    /// <summary>
    /// Ability State
    /// </summary>
    state,

    /// <summary>
    /// Ability Permission
    /// </summary>
    permission,
}

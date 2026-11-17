using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Batch abilities
/// </summary>
public sealed record BatchAbilities(
    [property: JsonPropertyName("cancel")] PingenApiAbility Cancel,
    [property: JsonPropertyName("delete")] PingenApiAbility Delete,
    [property: JsonPropertyName("submit")] PingenApiAbility Submit,
    [property: JsonPropertyName("edit")] PingenApiAbility Edit,
    [property: JsonPropertyName("change-window-position")] PingenApiAbility ChangeWindowPosition,
    [property: JsonPropertyName("add-attachment")] PingenApiAbility AddAttachment
) : IAbilities;

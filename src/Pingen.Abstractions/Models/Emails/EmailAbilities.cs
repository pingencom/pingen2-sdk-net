using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Emails;

/// <summary>
/// Email abilities (<c>meta.abilities.self</c>)
/// </summary>
public sealed record EmailAbilities(
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-raw")] PingenApiAbility GetPdfRaw,
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-validation")] PingenApiAbility GetPdfValidation,
    [property: JsonPropertyName("restore-original")] PingenApiAbility RestoreOriginal,
    [property: JsonPropertyName("delete")] PingenApiAbility Delete,
    [property: JsonPropertyName("cancel")] PingenApiAbility Cancel,
    [property: JsonPropertyName("apply-preset")] PingenApiAbility ApplyPreset,
    [property: JsonPropertyName("create-preset")] PingenApiAbility CreatePreset,
    [property: JsonPropertyName("revalidate")] PingenApiAbility Revalidate,
    [property: JsonPropertyName("add-attachment")] PingenApiAbility AddAttachment
) : IAbilities;

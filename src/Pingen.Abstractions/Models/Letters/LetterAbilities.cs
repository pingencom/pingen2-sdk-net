using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Letters;

/// <summary>
/// Letter abilities
/// </summary>
public sealed record LetterAbilities(
    [property: JsonPropertyName("cancel")] PingenApiAbility Cancel,
    [property: JsonPropertyName("delete")] PingenApiAbility Delete,
    [property: JsonPropertyName("submit")] PingenApiAbility Submit,
    [property: JsonPropertyName("send-simplex")] PingenApiAbility SendSimplex,
    [property: JsonPropertyName("edit")] PingenApiAbility Edit,
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-raw")] PingenApiAbility GetPdfRaw,
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-validation")] PingenApiAbility GetPdfValidation,
    [property: JsonPropertyName("change-paper-type")] PingenApiAbility ChangePaperType,
    [property: JsonPropertyName("change-window-position")] PingenApiAbility ChangeWindowPosition,
    [property: JsonPropertyName("create-coverpage")] PingenApiAbility CreateCoverPage,
    [property: JsonPropertyName("fix-overwrite-restricted-areas")] PingenApiAbility FixOverwriteRestrictedAreas,
    [property: JsonPropertyName("fix-coverpage")] PingenApiAbility FixCoverPage,
    [property: JsonPropertyName("fix-regular-paper")] PingenApiAbility FixRegularPaper
) : IAbilities;

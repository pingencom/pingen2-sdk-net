using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Ebill abilities (<c>meta.abilities.self</c>)
/// </summary>
public sealed record EbillAbilities(
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-raw")] PingenApiAbility GetPdfRaw,
    // ReSharper disable once InconsistentNaming
    [property: JsonPropertyName("get-pdf-validation")] PingenApiAbility GetPdfValidation,
    [property: JsonPropertyName("restore-original")] PingenApiAbility RestoreOriginal,
    [property: JsonPropertyName("delete")] PingenApiAbility Delete,
    [property: JsonPropertyName("cancel")] PingenApiAbility Cancel,
    [property: JsonPropertyName("submit")] PingenApiAbility Submit,
    [property: JsonPropertyName("apply-preset")] PingenApiAbility ApplyPreset,
    [property: JsonPropertyName("create-preset")] PingenApiAbility CreatePreset,
    [property: JsonPropertyName("revalidate")] PingenApiAbility Revalidate,
    [property: JsonPropertyName("add-attachment")] PingenApiAbility AddAttachment,
    [property: JsonPropertyName("fix-format")] PingenApiAbility FixFormat,
    [property: JsonPropertyName("define-qr")] PingenApiAbility DefineQr,
    [property: JsonPropertyName("define-invoice-date")] PingenApiAbility DefineInvoiceDate,
    [property: JsonPropertyName("define-invoice-due-date")] PingenApiAbility DefineInvoiceDueDate,
    [property: JsonPropertyName("define-recipient-identifier")] PingenApiAbility DefineRecipientIdentifier,
    [property: JsonPropertyName("define-invoice-number")] PingenApiAbility DefineInvoiceNumber
) : IAbilities;

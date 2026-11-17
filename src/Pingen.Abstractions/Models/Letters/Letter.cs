using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Letters.Embedded;

namespace Pingen.Abstractions.Models.Letters;

/// <summary>
/// Letter
/// </summary>
/// <param name="Status">Current status of the letter.</param>
/// <param name="DeliveryProduct">Delivery product the letter was sent with.</param>
/// <param name="Source">Where the letter came from. Any of <c>Letter.Source</c>.</param>
[PingenApiType("letters")]
public sealed record Letter(
    [property: JsonPropertyName(LetterFields.Status)] string? Status,
    [property: JsonPropertyName(LetterFields.FileOriginalName)] string? FileOriginalName,
    [property: JsonPropertyName(LetterFields.FilePages)] int? FilePages,
    [property: JsonPropertyName(LetterFields.Address)] string? Address,
    [property: JsonPropertyName(LetterFields.AddressPosition)] LetterAddressPosition? AddressPosition,
    [property: JsonPropertyName(LetterFields.Country)] string? Country,
    [property: JsonPropertyName(LetterFields.DeliveryProduct)] string? DeliveryProduct,
    [property: JsonPropertyName(LetterFields.PrintMode)] LetterPrintMode? PrintMode,
    [property: JsonPropertyName(LetterFields.PrintSpectrum)] LetterPrintSpectrum? PrintSpectrum,
    [property: JsonPropertyName(LetterFields.PriceCurrency)] string? PriceCurrency,
    [property: JsonPropertyName(LetterFields.PriceValue)] decimal? PriceValue,
    [property: JsonPropertyName(LetterFields.PaperTypes)] IReadOnlyList<string>? PaperTypes,
    [property: JsonPropertyName(LetterFields.Fonts)] IReadOnlyList<LetterFont>? Fonts,
    [property: JsonPropertyName(LetterFields.TrackingNumber)] string? TrackingNumber,
    [property: JsonPropertyName(LetterFields.SubmittedAt)] DateTime? SubmittedAt,
    [property: JsonPropertyName(LetterFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(LetterFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(LetterFields.Source)] string? Source = null
) : IAttributes;

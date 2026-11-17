using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.LetterPrices;

/// <summary>
/// Letter price
/// </summary>
[PingenApiType("letter_price_calculator")]
public sealed record LetterPrice(
    [property: JsonPropertyName(LetterPriceFields.Currency)] string? Currency,
    [property: JsonPropertyName(LetterPriceFields.Price)] decimal? Price
) : IAttributes;

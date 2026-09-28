using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.LetterPrices;

/// <summary>
/// Letter configuration for price calculator to send via <see cref="DataPost{TAttributes}"/> to the API
/// </summary>
public sealed record LetterPriceConfiguration : IAttributes
{
    /// <summary>
    /// Country
    /// </summary>
    [JsonPropertyName("country")]
    public required string Country { get; init; }

    /// <summary>
    /// Paper types. Use values from <c>paper_types</c>
    /// </summary>
    [JsonPropertyName("paper_types")]
    public required List<string> PaperTypes { get; init; }

    /// <summary>
    /// Print mode
    /// </summary>
    [JsonPropertyName("print_mode")]
    public required LetterPrintMode PrintMode { get; init; }

    /// <summary>
    /// Print spectrum
    /// </summary>
    [JsonPropertyName("print_spectrum")]
    public required LetterPrintSpectrum PrintSpectrum { get; init; }

    /// <summary>
    /// Delivery product to calculate the price for.
    /// </summary>
    [JsonPropertyName("delivery_product")]
    public required string DeliveryProduct { get; init; }
}

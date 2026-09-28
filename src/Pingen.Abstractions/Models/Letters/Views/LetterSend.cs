using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Letters.Embedded;

namespace Pingen.Abstractions.Models.Letters.Views;

/// <summary>
/// Letter send object to send via <see cref="DataPatch{TAttributes}"/> to the API
/// </summary>
public sealed record LetterSend : IAttributes
{
    /// <summary>
    /// Delivery product to send the letter with.
    /// </summary>
    [JsonPropertyName("delivery_product")]
    public required string DeliveryProduct { get; init; }

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
    /// Meta data
    /// <br/> NOTE: This must only be set when <c>postag_registered</c> or <c>postag_a_plus</c> product used. Otherwise the API can fail at address validation when Zip code has more than 4 characters.
    /// </summary>
    [JsonPropertyName("meta_data")]
    public LetterMetaData? MetaData { get; init; }
}

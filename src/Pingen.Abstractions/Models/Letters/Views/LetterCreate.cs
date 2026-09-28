using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Letters.Embedded;

namespace Pingen.Abstractions.Models.Letters.Views;

/// <summary>
/// Letter create object to send via <see cref="DataPost{TAttributes}"/> to the API
/// </summary>
public sealed record LetterCreate : IAttributes
{
    /// <summary>
    /// Filename [ 5 .. 255 ] characters
    /// </summary>
    [JsonPropertyName("file_original_name")]
    public required string FileOriginalName { get; init; }

    /// <summary>
    /// File URL [ 1 .. 1000 ] characters
    /// </summary>
    [JsonPropertyName("file_url")]
    public required string FileUrl { get; init; }

    /// <summary>
    /// File URL signature [ 1 .. 60 ] characters
    /// </summary>
    [JsonPropertyName("file_url_signature")]
    public required string FileUrlSignature { get; init; }

    /// <summary>
    /// Address position
    /// </summary>
    [JsonPropertyName("address_position")]
    public required LetterAddressPosition AddressPosition { get; init; }

    /// <summary>
    /// Auto send
    /// </summary>
    [JsonPropertyName("auto_send")]
    public required bool AutoSend { get; init; }

    /// <summary>
    /// Delivery product; any of <see cref="LetterCreateDeliveryProduct"/>. Optional: only required when
    /// <see cref="AutoSend"/> is <c>true</c>; a more specific product can be chosen later at Letters/Send.
    /// </summary>
    [JsonPropertyName("delivery_product")]
    public string? DeliveryProduct { get; init; }

    /// <summary>
    /// Print mode. Optional: only required when <see cref="AutoSend"/> is <c>true</c>.
    /// </summary>
    [JsonPropertyName("print_mode")]
    public LetterPrintMode? PrintMode { get; init; }

    /// <summary>
    /// Print spectrum. Optional: only required when <see cref="AutoSend"/> is <c>true</c>.
    /// </summary>
    [JsonPropertyName("print_spectrum")]
    public LetterPrintSpectrum? PrintSpectrum { get; init; }

    /// <summary>
    /// Meta data
    /// <br/> NOTE: This must only be set when <c>postag_registered</c> or <c>postag_a_plus</c> product used. Otherwise the API can fail at address validation when Zip code has more than 4 characters.
    /// </summary>
    [JsonPropertyName("meta_data")]
    public LetterMetaData? MetaData { get; init; }
}

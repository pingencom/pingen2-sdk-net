using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Letters.Embedded;

namespace Pingen.Abstractions.Models.Letters.Views;

/// <summary>
/// Input for the <c>UploadAndCreate</c> convenience flow - the letter create attributes without
/// <c>file_url</c> / <c>file_url_signature</c>, which are filled in automatically after the file is uploaded.
/// </summary>
public sealed record LetterUpload
{
    /// <summary>Filename [ 5 .. 255 ] characters.</summary>
    public required string FileOriginalName { get; init; }

    /// <summary>Address position.</summary>
    public required LetterAddressPosition AddressPosition { get; init; }

    /// <summary>Auto send.</summary>
    public required bool AutoSend { get; init; }

    /// <summary>Delivery product (any of <see cref="LetterCreateDeliveryProduct"/>). Optional: only required when <see cref="AutoSend"/> is <c>true</c>.</summary>
    public string? DeliveryProduct { get; init; }

    /// <summary>Print mode. Optional: only required when <see cref="AutoSend"/> is <c>true</c>.</summary>
    public LetterPrintMode? PrintMode { get; init; }

    /// <summary>Print spectrum. Optional: only required when <see cref="AutoSend"/> is <c>true</c>.</summary>
    public LetterPrintSpectrum? PrintSpectrum { get; init; }

    /// <summary>Optional meta data (recipient/sender).</summary>
    public LetterMetaData? MetaData { get; init; }
}

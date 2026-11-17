using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Input for the <c>UploadAndCreate</c> convenience flow - the batch create attributes without
/// <c>file_url</c> / <c>file_url_signature</c>, which are filled in automatically after the file is uploaded.
/// </summary>
public sealed record BatchUpload
{
    /// <summary>Name of the batch [ 5 .. 100 ] characters.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// Delivery channel - any of <see cref="BatchChannelTypes"/> (<c>post</c>, <c>ebill</c>, <c>email</c>).
    /// Optional; defaults to <c>post</c>.
    /// </summary>
    public string? ChannelType { get; init; }

    /// <summary>Icon.</summary>
    public required BatchIcon Icon { get; init; }

    /// <summary>Filename [ 5 .. 255 ] characters.</summary>
    public required string FileOriginalName { get; init; }

    /// <summary>Address position (postal batches). Optional.</summary>
    public LetterAddressPosition? AddressPosition { get; init; }

    /// <summary>Grouping type.</summary>
    public required BatchGroupingType GroupingType { get; init; }

    /// <summary>Grouping options split type.</summary>
    public required BatchGroupingOptionsSplitType GroupingOptionsSplitType { get; init; }

    /// <summary>Grouping options split size [ 1 .. 10 ].</summary>
    public int? GroupingOptionsSplitSize { get; init; }

    /// <summary>Grouping options split separator [ 1 .. 20 ] characters.</summary>
    public string? GroupingOptionsSplitSeparator { get; init; }

    /// <summary>Grouping options split position.</summary>
    public BatchGroupingOptionsSplitPosition? GroupingOptionsSplitPosition { get; init; }
}

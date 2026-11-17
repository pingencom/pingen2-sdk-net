using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Batch create object to send via <see cref="DataPost{TAttributes}"/> to the API
/// </summary>
public sealed record BatchCreate : IAttributes
{
    /// <summary>
    /// Name of the batch [ 5 .. 100 ] characters
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Delivery channel for the batch - any of <see cref="BatchChannelTypes"/> (<c>post</c>, <c>ebill</c>,
    /// <c>email</c>). Optional; the API defaults to <c>post</c> when omitted.
    /// </summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; init; }

    /// <summary>
    /// Icon
    /// </summary>
    [JsonPropertyName("icon")]
    public required BatchIcon Icon { get; init; }

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
    /// Address position (postal batches). Optional.
    /// </summary>
    [JsonPropertyName("address_position")]
    public LetterAddressPosition? AddressPosition { get; init; }

    /// <summary>
    /// Grouping type
    /// </summary>
    [JsonPropertyName("grouping_type")]
    public required BatchGroupingType GroupingType { get; init; }

    /// <summary>
    /// Grouping options type
    /// </summary>
    [JsonPropertyName("grouping_options_split_type")]
    public required BatchGroupingOptionsSplitType GroupingOptionsSplitType { get; init; }

    /// <summary>
    /// Grouping options split size [ 1 .. 10 ]
    /// </summary>
    [JsonPropertyName("grouping_options_split_size")]
    public int? GroupingOptionsSplitSize { get; init; }

    /// <summary>
    /// Grouping options split separator [ 1 .. 20 ] characters
    /// </summary>
    [JsonPropertyName("grouping_options_split_separator")]
    public string? GroupingOptionsSplitSeparator { get; init; }

    /// <summary>
    /// Grouping options split position
    /// </summary>
    [JsonPropertyName("grouping_options_split_position")]
    public BatchGroupingOptionsSplitPosition? GroupingOptionsSplitPosition { get; init; }
}

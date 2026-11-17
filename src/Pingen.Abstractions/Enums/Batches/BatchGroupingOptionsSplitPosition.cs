using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Batches;

/// <summary>
/// Batch grouping split position options
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BatchGroupingOptionsSplitPosition>))]
public enum BatchGroupingOptionsSplitPosition
{
    /// <summary>
    /// First page
    /// </summary>
    first_page,

    /// <summary>
    /// Last page
    /// </summary>
    last_page,
}

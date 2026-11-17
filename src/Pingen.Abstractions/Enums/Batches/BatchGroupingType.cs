using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Batches;

/// <summary>
/// Batch grouping type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BatchGroupingType>))]
public enum BatchGroupingType
{
    /// <summary>
    /// Batch is provided as an archive.
    /// </summary>
    zip,

    /// <summary>
    /// Batch is provided as a merged PDF of all letters.
    /// </summary>
    merge,
}

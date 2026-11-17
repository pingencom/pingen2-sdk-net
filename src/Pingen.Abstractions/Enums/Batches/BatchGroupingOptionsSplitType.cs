using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Batches;

/// <summary>
/// Batch grouping split type options
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BatchGroupingOptionsSplitType>))]
public enum BatchGroupingOptionsSplitType
{
    /// <summary>
    /// File
    /// </summary>
    file,

    /// <summary>
    /// Page
    /// </summary>
    page,

    /// <summary>
    /// Custom
    /// </summary>
    custom,

    /// <summary>
    /// QR-invoice
    /// </summary>
    qr_invoice,
}

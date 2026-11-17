using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Body attributes for deleting a batch. The Pingen batch-delete endpoint requires a JSON:API body that says
/// whether the contained deliverables should be deleted along with the batch.
/// </summary>
public sealed record BatchDelete : IAttributes
{
    /// <summary>
    /// Delete the letters contained in the batch as well.
    /// </summary>
    [JsonPropertyName("with_letters")]
    public required bool WithLetters { get; init; }

    /// <summary>
    /// Delete all deliverables contained in the batch as well (any channel).
    /// </summary>
    [JsonPropertyName("with_deliverables")]
    public required bool WithDeliverables { get; init; }
}

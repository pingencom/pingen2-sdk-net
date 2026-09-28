using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Batch
/// </summary>
/// <param name="DeliverableCount">Channel-agnostic item count; equal to <paramref name="LetterCount"/> for postal batches.</param>
/// <param name="ChannelType">Delivery channel of this batch. Any of <see cref="BatchChannelTypes"/>.</param>
/// <param name="Source">Where the batch came from. Any of <c>Batch.Source</c>.</param>
[PingenApiType("batches")]
public sealed record Batch(
    [property: JsonPropertyName(BatchFields.Name)] string? Name,
    [property: JsonPropertyName(BatchFields.Icon)] string? Icon,
    [property: JsonPropertyName(BatchFields.Status)] string? Status,
    [property: JsonPropertyName(BatchFields.FileOriginalName)] string? FileOriginalName,
    [property: JsonPropertyName(BatchFields.LetterCount)] int? LetterCount,
    [property: JsonPropertyName(BatchFields.AddressPosition)] LetterAddressPosition? AddressPosition,
    [property: JsonPropertyName(BatchFields.PrintMode)] LetterPrintMode? PrintMode,
    [property: JsonPropertyName(BatchFields.PrintSpectrum)] LetterPrintSpectrum? PrintSpectrum,
    [property: JsonPropertyName(BatchFields.PriceCurrency)] string? PriceCurrency,
    [property: JsonPropertyName(BatchFields.PriceValue)] decimal? PriceValue,
    [property: JsonPropertyName(BatchFields.SubmittedAt)] DateTime? SubmittedAt,
    [property: JsonPropertyName(BatchFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(BatchFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(BatchFields.DeliverableCount)] int? DeliverableCount = null,
    [property: JsonPropertyName(BatchFields.ChannelType)] string? ChannelType = null,
    [property: JsonPropertyName(BatchFields.Source)] string? Source = null
) : IAttributes;

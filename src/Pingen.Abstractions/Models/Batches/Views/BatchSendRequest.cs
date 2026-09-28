using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Builds the <see cref="DataPatch{TAttributes}"/> for sending a batch. A batch's channel decides both the
/// request's JSON:API <c>type</c> and which attributes apply, so use the matching factory method rather than
/// assembling the <see cref="DataPatch{TAttributes}"/> by hand - that keeps the type and attributes in sync.
/// </summary>
public static class BatchSendRequest
{
    /// <summary>
    /// Send a postal batch (<see cref="BatchChannelTypes.Post"/>) with a delivery product and print options.
    /// </summary>
    /// <param name="batchId">ID of the batch to send.</param>
    /// <param name="deliveryProduct">Postal delivery product - any of <c>delivery_product</c>.</param>
    /// <param name="printMode">Print mode.</param>
    /// <param name="printSpectrum">Print spectrum.</param>
    public static DataPatch<BatchSend> Post(string batchId, string deliveryProduct, LetterPrintMode printMode, LetterPrintSpectrum printSpectrum) =>
        new()
        {
            Id = batchId,
            Type = "batches_channel_post_send",
            Attributes = new BatchSend
            {
                DeliveryProduct = deliveryProduct,
                PrintMode = printMode,
                PrintSpectrum = printSpectrum,
            },
        };

    /// <summary>
    /// Send an email batch (<see cref="BatchChannelTypes.Email"/>).
    /// </summary>
    /// <param name="batchId">ID of the batch to send.</param>
    public static DataPatch<BatchSend> Email(string batchId) =>
        new()
        {
            Id = batchId,
            Type = "batches_channel_email_send",
            Attributes = new BatchSend { DeliveryProduct = "electronic_email" },
        };

    /// <summary>
    /// Send an ebill batch (<see cref="BatchChannelTypes.Ebill"/>).
    /// </summary>
    /// <param name="batchId">ID of the batch to send.</param>
    public static DataPatch<BatchSend> Ebill(string batchId) =>
        new()
        {
            Id = batchId,
            Type = "batches_channel_ebill_send",
            Attributes = new BatchSend { DeliveryProduct = "electronic_ebill" },
        };
}

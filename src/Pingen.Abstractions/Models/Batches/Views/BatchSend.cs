using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Batch send object to send via <see cref="DataPatch{TAttributes}"/> to the API. The enclosing
/// <see cref="DataPatch{TAttributes}"/> needs a channel-specific <c>type</c>, so build the whole payload with
/// <see cref="BatchSendRequest"/> rather than newing this up directly.
/// <para>
/// The required fields depend on the batch channel. For the postal channel provide
/// <see cref="DeliveryProduct"/> (e.g. <c>fast</c>, <c>cheap</c>, <c>registered</c>) together with
/// <see cref="PrintMode"/> and <see cref="PrintSpectrum"/>. For the email / ebill channels only
/// <see cref="DeliveryProduct"/> is used (<c>electronic_email</c> / <c>electronic_ebill</c>), and the print
/// options are left null.
/// </para>
/// </summary>
public sealed record BatchSend : IAttributes
{
    /// <summary>
    /// Delivery product. Postal: any of <c>delivery_product</c>. Email / ebill:
    /// <c>electronic_email</c> /
    /// <c>electronic_ebill</c>.
    /// </summary>
    [JsonPropertyName("delivery_product")]
    public required string DeliveryProduct { get; init; }

    /// <summary>
    /// Print mode (postal channel only).
    /// </summary>
    [JsonPropertyName("print_mode")]
    public LetterPrintMode? PrintMode { get; init; }

    /// <summary>
    /// Print spectrum (postal channel only).
    /// </summary>
    [JsonPropertyName("print_spectrum")]
    public LetterPrintSpectrum? PrintSpectrum { get; init; }
}

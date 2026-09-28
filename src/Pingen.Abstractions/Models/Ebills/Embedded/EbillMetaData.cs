using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Ebills.Views;

namespace Pingen.Abstractions.Models.Ebills.Embedded;

/// <summary>
/// Meta data for <see cref="EbillCreate"/>. Carries the invoice information used when the deliverable is sent
/// through the ebill channel.
/// </summary>
public sealed record EbillMetaData
{
    /// <summary>
    /// Invoice number.
    /// </summary>
    [JsonPropertyName("invoice_number")]
    public required string InvoiceNumber { get; init; }

    /// <summary>
    /// Invoice date, format <c>Y-m-d</c>.
    /// </summary>
    [JsonPropertyName("invoice_date")]
    public required string InvoiceDate { get; init; }

    /// <summary>
    /// Invoice due date, format <c>Y-m-d</c>.
    /// </summary>
    [JsonPropertyName("invoice_due_date")]
    public required string InvoiceDueDate { get; init; }

    /// <summary>
    /// Recipient identifier.
    /// </summary>
    [JsonPropertyName("recipient_identifier")]
    public required string RecipientIdentifier { get; init; }
}

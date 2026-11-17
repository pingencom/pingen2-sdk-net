using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Ebill deliverable. One of the three deliverable channels alongside letters and emails, living under
/// <c>deliveries/ebills</c>.
/// </summary>
/// <param name="InvoiceDate">Invoice date, format <c>Y-m-d</c>.</param>
/// <param name="InvoiceDueDate">Invoice due date, format <c>Y-m-d</c>.</param>
/// <param name="Source">Where the ebill came from. Any of <c>Letter.Source</c>.</param>
[PingenApiType("ebills")]
public sealed record Ebill(
    [property: JsonPropertyName(EbillFields.Status)] string? Status,
    [property: JsonPropertyName(EbillFields.FileOriginalName)] string? FileOriginalName,
    [property: JsonPropertyName(EbillFields.FilePages)] int? FilePages,
    [property: JsonPropertyName(EbillFields.RecipientIdentifier)] string? RecipientIdentifier,
    [property: JsonPropertyName(EbillFields.RecipientAddress)] string? RecipientAddress,
    [property: JsonPropertyName(EbillFields.InvoiceNumber)] string? InvoiceNumber,
    [property: JsonPropertyName(EbillFields.InvoiceDate)] string? InvoiceDate,
    [property: JsonPropertyName(EbillFields.InvoiceDueDate)] string? InvoiceDueDate,
    [property: JsonPropertyName(EbillFields.InvoiceValue)] decimal? InvoiceValue,
    [property: JsonPropertyName(EbillFields.InvoiceCurrency)] string? InvoiceCurrency,
    [property: JsonPropertyName(EbillFields.InvoiceIban)] string? InvoiceIban,
    [property: JsonPropertyName(EbillFields.InvoiceAddress)] string? InvoiceAddress,
    [property: JsonPropertyName(EbillFields.InvoiceReference)] string? InvoiceReference,
    [property: JsonPropertyName(EbillFields.PriceCurrency)] string? PriceCurrency,
    [property: JsonPropertyName(EbillFields.PriceValue)] decimal? PriceValue,
    [property: JsonPropertyName(EbillFields.SubmittedAt)] DateTime? SubmittedAt,
    [property: JsonPropertyName(EbillFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(EbillFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(EbillFields.Source)] string? Source = null
) : IAttributes;

using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Emails;

/// <summary>
/// Email deliverable. One of the three deliverable channels alongside letters and ebills, living under
/// <c>deliveries/emails</c>.
/// </summary>
/// <param name="Source">Where the email came from. Any of <c>Letter.Source</c>.</param>
[PingenApiType("emails")]
public sealed record Email(
    [property: JsonPropertyName(EmailFields.Status)] string? Status,
    [property: JsonPropertyName(EmailFields.FileOriginalName)] string? FileOriginalName,
    [property: JsonPropertyName(EmailFields.FilePages)] int? FilePages,
    [property: JsonPropertyName(EmailFields.RecipientIdentifier)] string? RecipientIdentifier,
    [property: JsonPropertyName(EmailFields.PriceCurrency)] string? PriceCurrency,
    [property: JsonPropertyName(EmailFields.PriceValue)] decimal? PriceValue,
    [property: JsonPropertyName(EmailFields.SubmittedAt)] DateTime? SubmittedAt,
    [property: JsonPropertyName(EmailFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(EmailFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(EmailFields.Source)] string? Source = null
) : IAttributes;

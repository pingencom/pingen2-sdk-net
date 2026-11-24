using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Organisations;

/// <summary>
/// Organisation
/// </summary>
/// <param name="Edition">Product edition the organisation is on.</param>
/// <param name="Flags">Feature flags enabled for the organisation.</param>
/// <param name="MissingCredits">
/// Credits the organisation is short of. Non-zero means prepaid sending stalls until it is topped up.
/// </param>
[PingenApiType("organisations")]
public sealed record Organisation(
    [property: JsonPropertyName(OrganisationFields.Name)] string? Name,
    [property: JsonPropertyName(OrganisationFields.Status)] string? Status,
    [property: JsonPropertyName(OrganisationFields.Plan)] string? Plan,
    [property: JsonPropertyName(OrganisationFields.BillingMode)] string? BillingMode,
    [property: JsonPropertyName(OrganisationFields.BillingCurrency)] string? BillingCurrency,
    [property: JsonPropertyName(OrganisationFields.BillingBalance)] decimal? BillingBalance,
    [property: JsonPropertyName(OrganisationFields.DefaultAddressPosition)] LetterAddressPosition? DefaultAddressPosition,
    [property: JsonPropertyName(OrganisationFields.DataRetentionAddresses)] int? DataRetentionAddresses,
    [property: JsonPropertyName(OrganisationFields.DataRetentionPdf)] int? DataRetentionPdf,
    [property: JsonPropertyName(OrganisationFields.Color)] string? Color,
    [property: JsonPropertyName(OrganisationFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(OrganisationFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(OrganisationFields.Edition)] string? Edition = null,
    [property: JsonPropertyName(OrganisationFields.Flags)] IReadOnlyList<string>? Flags = null,
    [property: JsonPropertyName(OrganisationFields.MissingCredits)] decimal? MissingCredits = null
) : IAttributes;

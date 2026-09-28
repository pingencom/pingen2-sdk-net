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
/// <param name="LimitsMonthlyLettersCount">Monthly letter quota.</param>
/// <param name="LimitsMonthlyEmailsCount">Monthly email quota.</param>
/// <param name="LimitsMonthlyEbillsCount">Monthly ebill quota.</param>
[PingenApiType("organisations")]
public sealed record Organisation(
    [property: JsonPropertyName(OrganisationFields.Name)] string? Name,
    [property: JsonPropertyName(OrganisationFields.Status)] string? Status,
    [property: JsonPropertyName(OrganisationFields.Plan)] string? Plan,
    [property: JsonPropertyName(OrganisationFields.BillingMode)] string? BillingMode,
    [property: JsonPropertyName(OrganisationFields.BillingCurrency)] string? BillingCurrency,
    [property: JsonPropertyName(OrganisationFields.BillingBalance)] decimal? BillingBalance,
    [property: JsonPropertyName(OrganisationFields.DefaultCountry)] string? DefaultCountry,
    [property: JsonPropertyName(OrganisationFields.DefaultAddressPosition)] LetterAddressPosition? DefaultAddressPosition,
    [property: JsonPropertyName(OrganisationFields.DataRetentionAddresses)] int? DataRetentionAddresses,
    [property: JsonPropertyName(OrganisationFields.DataRetentionPdf)] int? DataRetentionPdf,
    [property: JsonPropertyName(OrganisationFields.Color)] string? Color,
    [property: JsonPropertyName(OrganisationFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(OrganisationFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(OrganisationFields.Edition)] string? Edition = null,
    [property: JsonPropertyName(OrganisationFields.Flags)] IReadOnlyList<string>? Flags = null,
    [property: JsonPropertyName(OrganisationFields.MissingCredits)] decimal? MissingCredits = null,
    [property: JsonPropertyName(OrganisationFields.LimitsMonthlyLettersCount)] int? LimitsMonthlyLettersCount = null,
    [property: JsonPropertyName(OrganisationFields.LimitsMonthlyEmailsCount)] int? LimitsMonthlyEmailsCount = null,
    [property: JsonPropertyName(OrganisationFields.LimitsMonthlyEbillsCount)] int? LimitsMonthlyEbillsCount = null
) : IAttributes;

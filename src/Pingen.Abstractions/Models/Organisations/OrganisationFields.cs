namespace Pingen.Abstractions.Models.Organisations;

/// <summary>
/// Available sparse fieldset field names for the Organisation resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class OrganisationFields
{
    /// <summary>Organisation name</summary>
    public const string Name = "name";

    /// <summary>Organisation status</summary>
    public const string Status = "status";

    /// <summary>Subscription plan name</summary>
    public const string Plan = "plan";

    /// <summary>Billing mode (prepaid or postpaid)</summary>
    public const string BillingMode = "billing_mode";

    /// <summary>Billing currency code</summary>
    public const string BillingCurrency = "billing_currency";

    /// <summary>Current billing balance</summary>
    public const string BillingBalance = "billing_balance";

    /// <summary>Default country code for letters</summary>
    public const string DefaultCountry = "default_country";

    /// <summary>Default address window position</summary>
    public const string DefaultAddressPosition = "default_address_position";

    /// <summary>Address data retention period in days</summary>
    public const string DataRetentionAddresses = "data_retention_addresses";

    /// <summary>PDF data retention period in days</summary>
    public const string DataRetentionPdf = "data_retention_pdf";

    /// <summary>Organisation brand colour</summary>
    public const string Color = "color";

    /// <summary>Product edition the organisation is on</summary>
    public const string Edition = "edition";

    /// <summary>Feature flags enabled for the organisation</summary>
    public const string Flags = "flags";

    /// <summary>
    /// Credits the organisation is short of. Non-zero means prepaid sending will stall until it is topped up.
    /// </summary>
    public const string MissingCredits = "missing_credits";

    /// <summary>Monthly letter quota</summary>
    public const string LimitsMonthlyLettersCount = "limits_monthly_letters_count";

    /// <summary>Monthly email quota</summary>
    public const string LimitsMonthlyEmailsCount = "limits_monthly_emails_count";

    /// <summary>Monthly ebill quota</summary>
    public const string LimitsMonthlyEbillsCount = "limits_monthly_ebills_count";

    /// <summary>Timestamp when the organisation was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the organisation was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

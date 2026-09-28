namespace Pingen.Abstractions.Models.Emails;

/// <summary>
/// Available sparse fieldset field names for the Email resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class EmailFields
{
    /// <summary>Email status (e.g. validating, sent)</summary>
    public const string Status = "status";

    /// <summary>Original file name of the uploaded PDF</summary>
    public const string FileOriginalName = "file_original_name";

    /// <summary>Number of pages in the uploaded PDF</summary>
    public const string FilePages = "file_pages";

    /// <summary>Recipient identifier of the email delivery</summary>
    public const string RecipientIdentifier = "recipient_identifier";

    /// <summary>Currency code for the price</summary>
    public const string PriceCurrency = "price_currency";

    /// <summary>Price value for sending this email</summary>
    public const string PriceValue = "price_value";

    /// <summary>Where the email came from - one of <c>Letter.Source</c></summary>
    public const string Source = "source";

    /// <summary>Timestamp when the email was submitted for sending</summary>
    public const string SubmittedAt = "submitted_at";

    /// <summary>Timestamp when the email was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the email was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

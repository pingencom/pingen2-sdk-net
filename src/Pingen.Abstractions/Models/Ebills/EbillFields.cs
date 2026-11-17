namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Available sparse fieldset field names for the Ebill resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class EbillFields
{
    /// <summary>Ebill status</summary>
    public const string Status = "status";

    /// <summary>Original file name of the uploaded PDF</summary>
    public const string FileOriginalName = "file_original_name";

    /// <summary>Number of pages in the uploaded PDF</summary>
    public const string FilePages = "file_pages";

    /// <summary>Recipient identifier of the ebill delivery</summary>
    public const string RecipientIdentifier = "recipient_identifier";

    /// <summary>Recipient address</summary>
    public const string RecipientAddress = "recipient_address";

    /// <summary>Invoice number</summary>
    public const string InvoiceNumber = "invoice_number";

    /// <summary>Invoice date (Y-m-d)</summary>
    public const string InvoiceDate = "invoice_date";

    /// <summary>Invoice due date (Y-m-d)</summary>
    public const string InvoiceDueDate = "invoice_due_date";

    /// <summary>Invoice value</summary>
    public const string InvoiceValue = "invoice_value";

    /// <summary>Invoice currency</summary>
    public const string InvoiceCurrency = "invoice_currency";

    /// <summary>Invoice IBAN</summary>
    public const string InvoiceIban = "invoice_iban";

    /// <summary>Invoice address</summary>
    public const string InvoiceAddress = "invoice_address";

    /// <summary>Invoice reference</summary>
    public const string InvoiceReference = "invoice_reference";

    /// <summary>Currency code for the price</summary>
    public const string PriceCurrency = "price_currency";

    /// <summary>Price value for sending this ebill</summary>
    public const string PriceValue = "price_value";

    /// <summary>Where the ebill came from - one of <c>Letter.Source</c></summary>
    public const string Source = "source";

    /// <summary>Timestamp when the ebill was submitted for sending</summary>
    public const string SubmittedAt = "submitted_at";

    /// <summary>Timestamp when the ebill was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the ebill was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

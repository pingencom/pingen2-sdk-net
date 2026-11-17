namespace Pingen.Abstractions.Models.Letters;

/// <summary>
/// Available sparse fieldset field names for the Letter resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class LetterFields
{
    /// <summary>Letter status (e.g. valid, sending, sent)</summary>
    public const string Status = "status";

    /// <summary>Original file name of the uploaded PDF</summary>
    public const string FileOriginalName = "file_original_name";

    /// <summary>Number of pages in the uploaded PDF</summary>
    public const string FilePages = "file_pages";

    /// <summary>Full address string extracted from the letter</summary>
    public const string Address = "address";

    /// <summary>Position of the address window on the letter</summary>
    public const string AddressPosition = "address_position";

    /// <summary>Destination country code (ISO 3166-1 alpha-2)</summary>
    public const string Country = "country";

    /// <summary>Delivery product identifier</summary>
    public const string DeliveryProduct = "delivery_product";

    /// <summary>Print mode (simplex or duplex)</summary>
    public const string PrintMode = "print_mode";

    /// <summary>Print spectrum (color or grayscale)</summary>
    public const string PrintSpectrum = "print_spectrum";

    /// <summary>Currency code for the price</summary>
    public const string PriceCurrency = "price_currency";

    /// <summary>Price value for sending this letter</summary>
    public const string PriceValue = "price_value";

    /// <summary>List of paper types used</summary>
    public const string PaperTypes = "paper_types";

    /// <summary>List of fonts detected in the letter</summary>
    public const string Fonts = "fonts";

    /// <summary>Tracking number assigned to the letter</summary>
    public const string TrackingNumber = "tracking_number";

    /// <summary>Where the letter came from - one of <c>Letter.Source</c></summary>
    public const string Source = "source";

    /// <summary>Timestamp when the letter was submitted for sending</summary>
    public const string SubmittedAt = "submitted_at";

    /// <summary>Timestamp when the letter was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the letter was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Available sparse fieldset field names for the Batch resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class BatchFields
{
    /// <summary>Batch name</summary>
    public const string Name = "name";

    /// <summary>Batch icon identifier</summary>
    public const string Icon = "icon";

    /// <summary>Batch status</summary>
    public const string Status = "status";

    /// <summary>Original file name of the batch PDF</summary>
    public const string FileOriginalName = "file_original_name";

    /// <summary>Number of letters in this batch</summary>
    public const string LetterCount = "letter_count";

    /// <summary>
    /// Number of deliverables in this batch - the channel-agnostic counterpart to <see cref="LetterCount"/>;
    /// equal to it for postal batches.
    /// </summary>
    public const string DeliverableCount = "deliverable_count";

    /// <summary>
    /// Delivery channel of this batch - one of <see cref="Enums.Batches.BatchChannelTypes"/>.
    /// </summary>
    public const string ChannelType = "channel_type";

    /// <summary>Where the batch came from - one of <c>Batch.Source</c></summary>
    public const string Source = "source";

    /// <summary>Address position for letters in this batch</summary>
    public const string AddressPosition = "address_position";

    /// <summary>Print mode for letters in this batch</summary>
    public const string PrintMode = "print_mode";

    /// <summary>Print spectrum for letters in this batch</summary>
    public const string PrintSpectrum = "print_spectrum";

    /// <summary>Currency code for the batch price</summary>
    public const string PriceCurrency = "price_currency";

    /// <summary>Total price value for the batch</summary>
    public const string PriceValue = "price_value";

    /// <summary>Timestamp when the batch was submitted</summary>
    public const string SubmittedAt = "submitted_at";

    /// <summary>Timestamp when the batch was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the batch was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

namespace Pingen.Abstractions.Enums.Letters;

/// <summary>
/// Delivery products for the Letters/Create endpoint. A more specific product can be chosen later at
/// Letters/Send (see <c>delivery_product</c>).
/// </summary>
public static class LetterCreateDeliveryProduct
{
    /// <summary>
    /// Fast delivery product.
    /// </summary>
    public const string Fast = "fast";

    /// <summary>
    /// Cheap delivery product.
    /// </summary>
    public const string Cheap = "cheap";

    /// <summary>
    /// Bulk delivery product.
    /// </summary>
    public const string Bulk = "bulk";

    /// <summary>
    /// Premium delivery product.
    /// </summary>
    public const string Premium = "premium";

    /// <summary>
    /// Registered delivery product.
    /// </summary>
    public const string Registered = "registered";
}

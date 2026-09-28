namespace Pingen.Abstractions.Models.LetterPrices;

/// <summary>
/// Available sparse fieldset field names for the LetterPrice resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class LetterPriceFields
{
    /// <summary>Currency of the price</summary>
    public const string Currency = "currency";

    /// <summary>Price amount</summary>
    public const string Price = "price";
}

namespace Pingen.Abstractions.Constants;

/// <summary>
/// API query parameters for requests
/// </summary>
public static class ApiQueryParameterNames
{
    /// <summary>
    /// Query parameter for Sorting
    /// </summary>
    public const string Sorting = "sort";

    /// <summary>
    /// Query parameter for Filtering
    /// </summary>
    public const string Filtering = "filter";

    /// <summary>
    /// Query parameter for Searching
    /// </summary>
    public const string Searching = "q";

    /// <summary>
    /// Query parameter for page number
    /// </summary>
    public const string PageNumber = "page[number]";

    /// <summary>
    /// Query parameter for page limit
    /// </summary>
    public const string PageLimit = "page[limit]";

    /// <summary>
    /// Query parameter for sparse fieldsets
    /// </summary>
    public static string SparseFields(string type) => $"fields[{type}]";

    /// <summary>
    /// Query parameter for Include
    /// </summary>
    public const string Include = "include";

    /// <summary>
    /// Query parameter for language
    /// </summary>
    public const string Language = "language";
}

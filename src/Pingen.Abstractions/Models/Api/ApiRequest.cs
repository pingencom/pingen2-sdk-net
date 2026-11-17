namespace Pingen.Abstractions.Models.Api;

/// <summary>
/// An API request object to sent to the API with meta information to send as headers or query parameters
/// </summary>
public record ApiRequest
{
    /// <summary>
    /// Sparse fieldsets allow clients to request only specific fields in the response, reducing payload size.
    /// Each entry maps a <c>type</c> to the field names to include.
    /// Serialized as <c>fields[type]=field1,field2</c> query parameters per JSON:API specification.
    /// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
    /// </summary>
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? SparseFieldsets { get; init; }

    /// <summary>
    /// Enumerable of relationship names to include in the response (JSON:API include parameter).
    /// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
    /// </summary>
    public IEnumerable<string>? Include { get; init; }
}

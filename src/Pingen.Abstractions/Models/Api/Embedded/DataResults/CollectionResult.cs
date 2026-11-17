using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.DataResults.Embedded;

namespace Pingen.Abstractions.Models.Api.Embedded.DataResults;

/// <summary>
/// Generic response of collection endpoint
/// </summary>
/// <param name="Data">Collection of data objects requested from endpoint</param>
/// <param name="Links">Collection result links</param>
/// <param name="Meta">Collection result meta information</param>
public sealed record CollectionResult<TData>(
    [property: JsonPropertyName("data")] IList<TData> Data,
    [property: JsonPropertyName("links")] CollectionResultLinks Links,
    [property: JsonPropertyName("meta")] CollectionResultMeta Meta
) : IDataResult<IList<TData>>
    where TData : IData
{
    /// <summary>
    /// Additionally requested includes
    /// </summary>
    [JsonPropertyName("included")]
    public IncludedCollection? Included { get; init; }
}

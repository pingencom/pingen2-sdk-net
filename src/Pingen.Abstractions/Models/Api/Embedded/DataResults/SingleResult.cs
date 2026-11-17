using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Api.Embedded.DataResults;

/// <summary>
/// Generic response of single object endpoint
/// </summary>
/// <param name="Data">Data object requested from endpoint</param>
public sealed record SingleResult<TData>([property: JsonPropertyName("data")] TData Data) : IDataResult<TData>
    where TData : IData
{
    /// <summary>
    /// Additionally requested includes
    /// </summary>
    [JsonPropertyName("included")]
    public IncludedCollection? Included { get; init; }
}

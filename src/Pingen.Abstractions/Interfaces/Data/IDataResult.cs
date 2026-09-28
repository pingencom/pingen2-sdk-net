using Pingen.Abstractions.Models.Api.Embedded.DataResults;

namespace Pingen.Abstractions.Interfaces.Data;

/// <summary>
/// Empty interface to require an IDataResult{T} without type specification
/// </summary>
public interface IDataResult
{
    /// <summary>
    /// Optionally included data objects from API
    /// </summary>
    public IncludedCollection? Included { get; init; }
}

/// <summary>
/// Interface for all API data results
/// </summary>
public interface IDataResult<T> : IDataResult
{
    /// <summary>
    /// Data received from API
    /// </summary>
    public T Data { get; init; }
}

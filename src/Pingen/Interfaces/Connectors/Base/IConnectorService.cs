using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;

namespace Pingen.Interfaces.Connectors.Base;

/// <summary>
/// Base interface for all connector services for the API
/// </summary>
public interface IConnectorService
{
    /// <summary>
    /// Handle API result, throw on error, return data from <see cref="CollectionResult{TData}"/>
    /// </summary>
    /// <returns>List of data from collection result</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public IList<TData> HandleResult<TData>(ApiResult<CollectionResult<TData>> apiResult)
        where TData : IData;

    /// <summary>
    /// Handle API result, throw on error or missing data, return the data from <see cref="SingleResult{TData}"/>
    /// </summary>
    /// <returns>The single data object; never <c>null</c> (throws instead)</returns>
    /// <exception cref="PingenApiErrorException"></exception>
    public TData HandleResult<TData>(ApiResult<SingleResult<TData>> apiResult)
        where TData : IData;
}

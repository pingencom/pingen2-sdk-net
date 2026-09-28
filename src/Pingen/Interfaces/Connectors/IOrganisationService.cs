using System.Runtime.InteropServices;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen organisation service endpoint. <see href="https://api.pingen.com/documentation#tag/organisations.general">API Doc - Organisations General</see>
/// </summary>
public interface IOrganisationService : IConnectorService
{
    /// <summary>
    /// Get collection of organisations. <see href="https://api.pingen.com/documentation#tag/organisations.general/operation/organisations.index">API Doc - Organisations list</see>
    /// </summary>>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<OrganisationData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<OrganisationData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get details of an organisation. <see href="https://api.pingen.com/documentation#tag/organisations.general/operation/organisations.show">API Doc - Organisations show</see>
    /// </summary>
    /// <param name="organisationId">ID of the organisation to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<OrganisationDataDetailed>>> Get(
        string organisationId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );
}

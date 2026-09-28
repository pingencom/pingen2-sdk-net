using System.Runtime.InteropServices;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Users;
using Pingen.Interfaces.Connectors.Base;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen user service endpoint. <see href="https://api.pingen.com/documentation#tag/user.general">API Doc - Users General</see>
/// </summary>
public interface IUserService : IConnectorService
{
    /// <summary>
    /// Get details of the authenticated user. <see href="https://api.pingen.com/documentation#tag/user.general/operation/users.show">API Doc - Users show</see>
    /// </summary>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<UserDataDetailed>>> Get([Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Get collection of associations. <see href="https://api.pingen.com/documentation#tag/user.associations/operation/user.associations.list">API Doc - Users associations</see>
    /// </summary>>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<CollectionResult<UserAssociationDataDetailed>>> GetAssociationsPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Call <see cref="GetAssociationsPage"/> and auto page until end of collection
    /// </summary>
    /// <param name="apiPagingRequest">Optional, Request meta information to send to the API (where page number is the first page to start auto paging until end of collection)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <exception cref="PingenApiErrorException"></exception>
    public IAsyncEnumerable<IEnumerable<UserAssociationDataDetailed>> GetAssociationsPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    );

    /// <summary>
    /// Get details of a single user association.
    /// <br/>NOTE: Not part of the currently published OpenAPI spec; supported on a best-effort basis.
    /// </summary>
    /// <param name="associationId">ID of the association to get</param>
    /// <param name="apiRequest">Optional, Request meta information to send to the API (e.g. sparse fieldsets)</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<UserAssociationDataDetailed>>> GetAssociation(
        string associationId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    );
}

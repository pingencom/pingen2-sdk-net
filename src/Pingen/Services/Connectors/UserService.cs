using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Users;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IUserService" />
public sealed class UserService : ConnectorService, IUserService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    public UserService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<UserDataDetailed>>> Get([Optional] ApiRequest? apiRequest, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync<SingleResult<UserDataDetailed>>(
            UsersEndpoints.Root,
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<UserAssociationDataDetailed>>> GetAssociationsPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<UserAssociationDataDetailed>>(
            UsersEndpoints.Associations,
            apiPagingRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<UserAssociationDataDetailed>> GetAssociationsPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetAssociationsPage(apiRequest, cancellationToken))
                .WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<UserAssociationDataDetailed>>> GetAssociation(
        string associationId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<UserAssociationDataDetailed>>(
            requestPath: UsersEndpoints.Association(associationId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }
}

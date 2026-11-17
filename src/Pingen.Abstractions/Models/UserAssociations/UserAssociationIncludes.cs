namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// Available include relationship names for the UserAssociation resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class UserAssociationIncludes
{
    /// <summary>
    /// Include the organisation associated with this user association.
    /// </summary>
    public const string Organisation = "organisation";
}

namespace Pingen.Abstractions.Models.UserAssociations;

/// <summary>
/// Available sparse fieldset field names for the UserAssociation resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class UserAssociationFields
{
    /// <summary>User role within the organisation</summary>
    public const string Role = "role";

    /// <summary>Association status</summary>
    public const string Status = "status";

    /// <summary>Timestamp when the association was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the association was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

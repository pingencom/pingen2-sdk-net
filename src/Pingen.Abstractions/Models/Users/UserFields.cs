namespace Pingen.Abstractions.Models.Users;

/// <summary>
/// Available sparse fieldset field names for the User resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class UserFields
{
    /// <summary>User's email address</summary>
    public const string Email = "email";

    /// <summary>User's first name</summary>
    public const string FirstName = "first_name";

    /// <summary>User's last name</summary>
    public const string LastName = "last_name";

    /// <summary>User account status</summary>
    public const string Status = "status";

    /// <summary>User's preferred language code</summary>
    public const string Language = "language";

    /// <summary>Product edition the user is on</summary>
    public const string Edition = "edition";

    /// <summary>Feature flags enabled for the user</summary>
    public const string Flags = "flags";

    /// <summary>Timestamp when the user was created</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the user was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

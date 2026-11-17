namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for user service
/// </summary>
internal static class UsersEndpoints
{
    /// <summary>
    /// Root path of user
    /// </summary>
    internal const string Root = "user";

    /// <summary>
    /// Endpoint to get associations (page)
    /// </summary>
    internal const string Associations = $"{Root}/associations";

    /// <summary>
    /// Endpoint to access a single user association
    /// </summary>
    internal static string Association(string id) => $"{Associations}/{id}";
}

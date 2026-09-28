namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for organisations service
/// </summary>
internal static class OrganisationsEndpoints
{
    /// <summary>
    /// Root path of organisations
    /// </summary>
    internal const string Root = "organisations";

    /// <summary>
    /// Endpoint to access a single organisation
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";
}

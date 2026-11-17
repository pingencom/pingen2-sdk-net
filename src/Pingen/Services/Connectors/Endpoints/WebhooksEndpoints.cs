namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for webhooks service
/// </summary>
internal static class WebhooksEndpoints
{
    /// <summary>
    /// Root path of webhooks
    /// </summary>
    internal const string Root = "webhooks";

    /// <summary>
    /// Endpoint to access a single webhook
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";
}

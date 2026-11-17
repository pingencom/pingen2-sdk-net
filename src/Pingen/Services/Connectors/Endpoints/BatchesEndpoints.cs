namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for letters service
/// </summary>
internal static class BatchesEndpoints
{
    /// <summary>
    /// Root path of distributions
    /// </summary>
    internal const string Root = "batches";

    /// <summary>
    /// Endpoint to access a specific batch
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";

    /// <summary>
    /// Endpoint to send a specific batch
    /// </summary>
    internal static string Send(string id) => $"{Single(id)}/send";

    /// <summary>
    /// Endpoint to cancel a specific batch
    /// </summary>
    internal static string Cancel(string id) => $"{Single(id)}/cancel";

    /// <summary>
    /// Endpoint to get statistics of a specific batch
    /// </summary>
    internal static string Statistics(string id) => $"{Single(id)}/statistics";

    /// <summary>
    /// Endpoint to get events of a specific batch
    /// </summary>
    internal static string Events(string id, string language) => $"{Single(id)}/events?language={Uri.EscapeDataString(language)}";

    /// <summary>
    /// Endpoint to add an attachment to a specific batch
    /// </summary>
    internal static string Attachment(string id) => $"{Single(id)}/attachment";
}

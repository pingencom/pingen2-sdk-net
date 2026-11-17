namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for the emails service. Paths sit under <c>deliveries/emails</c>, the email
/// delivery channel sibling of letters and ebills.
/// </summary>
internal static class EmailsEndpoints
{
    /// <summary>
    /// Root path of email deliveries
    /// </summary>
    internal const string Root = "deliveries/emails";

    /// <summary>
    /// Endpoint to access a specific email
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";

    /// <summary>
    /// Endpoint to cancel a specific email
    /// </summary>
    internal static string Cancel(string id) => $"{Single(id)}/cancel";

    /// <summary>
    /// Endpoint to get the file of a specific email
    /// </summary>
    internal static string File(string id) => $"{Single(id)}/file";

    /// <summary>
    /// Endpoint to get events of a specific email
    /// </summary>
    internal static string Events(string id, string language) => $"{Single(id)}/events?language={Uri.EscapeDataString(language)}";

    /// <summary>
    /// Endpoint to get the image of a specific email event (302 redirect to the file URL).
    /// </summary>
    internal static string EventImage(string id, string eventId) => $"{Single(id)}/events/{eventId}/image";

    /// <summary>
    /// Endpoint to add an attachment to a specific email
    /// </summary>
    internal static string Attachment(string id) => $"{Single(id)}/attachment";
}

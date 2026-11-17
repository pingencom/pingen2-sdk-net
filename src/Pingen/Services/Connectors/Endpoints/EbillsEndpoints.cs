namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for the ebills service. Paths sit under <c>deliveries/ebills</c>, the ebill
/// delivery channel sibling of letters and emails.
/// </summary>
internal static class EbillsEndpoints
{
    /// <summary>
    /// Root path of ebill deliveries
    /// </summary>
    internal const string Root = "deliveries/ebills";

    /// <summary>
    /// Endpoint to access a specific ebill
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";

    /// <summary>
    /// Endpoint to send a specific ebill
    /// </summary>
    internal static string Send(string id) => $"{Single(id)}/send";

    /// <summary>
    /// Endpoint to cancel a specific ebill
    /// </summary>
    internal static string Cancel(string id) => $"{Single(id)}/cancel";

    /// <summary>
    /// Endpoint to get the file of a specific ebill
    /// </summary>
    internal static string File(string id) => $"{Single(id)}/file";

    /// <summary>
    /// Endpoint to get events of a specific ebill
    /// </summary>
    internal static string Events(string id, string language) => $"{Single(id)}/events?language={Uri.EscapeDataString(language)}";

    /// <summary>
    /// Endpoint to get the image of a specific ebill event (302 redirect to the file URL).
    /// </summary>
    internal static string EventImage(string id, string eventId) => $"{Single(id)}/events/{eventId}/image";

    /// <summary>
    /// Endpoint to add an attachment to a specific ebill
    /// </summary>
    internal static string Attachment(string id) => $"{Single(id)}/attachment";
}

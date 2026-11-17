namespace Pingen.Services.Connectors.Endpoints;

/// <summary>
/// Endpoints (API request paths) for the letters service. Paths sit under <c>deliveries/</c>, the documented
/// container shared by letters, emails and ebills.
/// </summary>
internal static class LettersEndpoints
{
    /// <summary>
    /// Root path of letter deliveries
    /// </summary>
    internal const string Root = "deliveries/letters";

    /// <summary>
    /// Endpoint to calculate price
    /// </summary>
    internal const string PriceCalculator = $"{Root}/price-calculator";

    /// <summary>
    /// Endpoint to access a specific letter
    /// </summary>
    internal static string Single(string id) => $"{Root}/{id}";

    /// <summary>
    /// Endpoint to send a specific letter
    /// </summary>
    internal static string Send(string id) => $"{Single(id)}/send";

    /// <summary>
    /// Endpoint to cancel a specific letter
    /// </summary>
    internal static string Cancel(string id) => $"{Single(id)}/cancel";

    /// <summary>
    /// Endpoint to get file of a specific letter
    /// </summary>
    internal static string File(string id) => $"{Single(id)}/file";

    /// <summary>
    /// Endpoint to add an attachment to a specific letter
    /// </summary>
    internal static string Attachment(string id) => $"{Single(id)}/attachment";

    /// <summary>
    /// Endpoint to add an attachment to multiple letters at once
    /// </summary>
    internal static string AttachmentMultiple => $"{Root}/attachment";

    /// <summary>
    /// Endpoint to get events of a specific letter
    /// </summary>
    internal static string Events(string id, string language) => $"{Single(id)}/events?language={Uri.EscapeDataString(language)}";

    /// <summary>
    /// Endpoint to get the image of a specific letter event (302 redirect to the file URL).
    /// </summary>
    internal static string EventImage(string id, string eventId) => $"{Single(id)}/events/{eventId}/image";

    /// <summary>
    /// Endpoint to get issues across all letters. One of four event-category collections under
    /// <c>{Root}/events/</c>; note the <c>events/</c> segment.
    /// </summary>
    internal static string Issues(string language) => EventCategory("issues", language);

    /// <summary>
    /// Endpoint to get sent events across all letters. One of four event-category collections under
    /// <c>{Root}/events/</c>.
    /// </summary>
    internal static string Sent(string language) => EventCategory("sent", language);

    /// <summary>
    /// Endpoint to get undeliverable events across all letters. One of four event-category collections under
    /// <c>{Root}/events/</c>.
    /// </summary>
    internal static string Undeliverable(string language) => EventCategory("undeliverable", language);

    /// <summary>
    /// Endpoint to get delivered events across all letters. One of four event-category collections under
    /// <c>{Root}/events/</c>.
    /// </summary>
    internal static string Delivered(string language) => EventCategory("delivered", language);

    /// <summary>
    /// Build an event-category collection path (<c>issues</c>, <c>sent</c>, <c>undeliverable</c>,
    /// <c>delivered</c>) under <c>{Root}/events/</c>.
    /// </summary>
    private static string EventCategory(string category, string language) => $"{Root}/events/{category}?language={Uri.EscapeDataString(language)}";
}

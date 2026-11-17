namespace Pingen.Abstractions.Models.LetterEvents;

/// <summary>
/// Available sparse fieldset field names for the LetterEvent resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class LetterEventFields
{
    /// <summary>Event code</summary>
    public const string Code = "code";

    /// <summary>Human-readable event name</summary>
    public const string Name = "name";

    /// <summary>Event producer (postal carrier etc.)</summary>
    public const string Producer = "producer";

    /// <summary>Physical location associated with the event</summary>
    public const string Location = "location";

    /// <summary>Whether the event has an associated image</summary>
    public const string HasImage = "has_image";

    /// <summary>Additional event data payload</summary>
    public const string Data = "data";

    /// <summary>Timestamp when the event was emitted by the carrier</summary>
    public const string EmittedAt = "emitted_at";

    /// <summary>Timestamp when the event was created in Pingen</summary>
    public const string CreatedAt = "created_at";

    /// <summary>Timestamp when the event was last updated</summary>
    public const string UpdatedAt = "updated_at";
}

namespace Pingen.Abstractions.Models.LetterEvents;

/// <summary>
/// Available include relationship names for the LetterEvent resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class LetterEventIncludes
{
    /// <summary>
    /// Include the letter associated with this event.
    /// </summary>
    public const string Letter = "letter";

    /// <summary>
    /// Include the deliverable (letter, email or ebill) associated with this event. Prefer this over
    /// <see cref="Letter"/>, which the letter-events endpoint may still return instead.
    /// </summary>
    public const string Deliverable = "deliverable";
}

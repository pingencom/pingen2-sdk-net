namespace Pingen.Abstractions.Models.Letters;

/// <summary>
/// Available include relationship names for the Letter resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class LetterIncludes
{
    /// <summary>
    /// Include the organisation that owns the letter.
    /// </summary>
    public const string Organisation = "organisation";

    /// <summary>
    /// Include the batch the letter belongs to.
    /// </summary>
    public const string Batch = "batch";
}

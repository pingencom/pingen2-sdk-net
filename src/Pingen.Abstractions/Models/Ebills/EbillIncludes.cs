namespace Pingen.Abstractions.Models.Ebills;

/// <summary>
/// Available include relationship names for the Ebill resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class EbillIncludes
{
    /// <summary>
    /// Include the organisation that owns the ebill.
    /// </summary>
    public const string Organisation = "organisation";

    /// <summary>
    /// Include the batch the ebill belongs to.
    /// </summary>
    public const string Batch = "batch";
}

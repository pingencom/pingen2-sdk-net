namespace Pingen.Abstractions.Models.Batches;

/// <summary>
/// Available include relationship names for the Batch resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class BatchIncludes
{
    /// <summary>
    /// Include the organisation that owns the batch.
    /// </summary>
    public const string Organisation = "organisation";
}

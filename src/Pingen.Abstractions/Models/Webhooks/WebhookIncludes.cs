namespace Pingen.Abstractions.Models.Webhooks;

/// <summary>
/// Available include relationship names for the Webhook resource.
/// Pass one or more of these constants to <see cref="Api.ApiRequest.Include"/>
/// to receive related resources sideloaded in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Including-relationships">API Doc - Including relationships</see>
/// </summary>
public static class WebhookIncludes
{
    /// <summary>
    /// Include the organisation that owns the webhook.
    /// </summary>
    public const string Organisation = "organisation";
}

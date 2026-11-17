namespace Pingen.Abstractions.Models.Webhooks;

/// <summary>
/// Available sparse fieldset field names for the Webhook resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class WebhookFields
{
    /// <summary>Webhook event category filter</summary>
    public const string EventCategory = "event_category";

    /// <summary>Webhook delivery URL</summary>
    public const string Url = "url";

    /// <summary>HMAC signing key for webhook signature validation</summary>
    public const string SigningKey = "signing_key";
}

namespace Pingen.Abstractions.Models.Webhooks.WebhookEvents;

/// <summary>
/// Available sparse fieldset field names for the WebhookEvent resource.
/// Pass one or more of these constants as field values in <see cref="Api.ApiRequest.SparseFieldsets"/>
/// to request only specific attributes in the response.
/// <see href="https://api.pingen.com/documentation#section/Advanced/Sparse-fieldsets">API Doc - Sparse fieldsets</see>
/// </summary>
public static class WebhookEventFields
{
    /// <summary>Reason for this webhook event (e.g. failure reason)</summary>
    public const string Reason = "reason";

    /// <summary>
    /// The address Pingen worked out for an undeliverable item. Spec-required on
    /// <c>WebhookDeliverableUndeliverableAttributes</c>, absent on every other event category.
    /// </summary>
    public const string CorrectedAddress = "corrected_address";

    /// <summary>Target URL the webhook was sent to</summary>
    public const string Url = "url";

    /// <summary>Timestamp when the webhook event was created</summary>
    public const string CreatedAt = "created_at";
}

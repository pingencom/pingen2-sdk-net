using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Webhooks.WebhookEvents.Embedded;

/// <summary>
/// The address Pingen worked out for an item it could not deliver, sent on <c>undeliverable</c> webhooks as
/// <c>data.attributes.corrected_address</c>.
/// </summary>
/// <param name="Name">Recipient name as corrected.</param>
/// <param name="Street">Street name, without the house number.</param>
/// <param name="Number">House number, e.g. <c>50A</c> - a string, not an integer.</param>
/// <param name="Zip">Postal code. A string: leading zeros and non-Swiss formats are both real.</param>
/// <param name="City">City name.</param>
public sealed record WebhookEventCorrectedAddress(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("street")] string? Street,
    [property: JsonPropertyName("number")] string? Number,
    [property: JsonPropertyName("zip")] string? Zip,
    [property: JsonPropertyName("city")] string? City
);

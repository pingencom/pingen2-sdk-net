using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.Relations;

namespace Pingen.Abstractions.Models.Webhooks;

/// <summary>
/// Webhook relationships
/// </summary>
public sealed record WebhookRelationships([property: JsonPropertyName("organisation")] RelatedSingleOutput Organisation) : IRelationships;

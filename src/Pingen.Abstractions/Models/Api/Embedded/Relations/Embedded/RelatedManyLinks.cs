using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;

/// <summary>
/// Related many links
/// </summary>
public sealed record RelatedManyLinks([property: JsonPropertyName("related")] RelatedManyLinkInfo Related);

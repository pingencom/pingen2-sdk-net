using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;

/// <summary>
/// Related many link info
/// </summary>
public sealed record RelatedManyLinkInfo([property: JsonPropertyName("href")] string Href, [property: JsonPropertyName("meta")] RelatedManyLinkMeta Meta);

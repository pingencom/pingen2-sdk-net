using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;

/// <summary>
/// Related many link meta
/// </summary>
public sealed record RelatedManyLinkMeta([property: JsonPropertyName("count")] int Count);

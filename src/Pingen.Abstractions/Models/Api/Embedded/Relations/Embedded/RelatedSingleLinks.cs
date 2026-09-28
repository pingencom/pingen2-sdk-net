using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;

/// <summary>
/// Relation links
/// </summary>
public sealed record RelatedSingleLinks([property: JsonPropertyName("related")] string Related);

using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations;

/// <summary>
/// A to-many relationship's output: a links object pointing at the related collection.
/// </summary>
public sealed record RelatedManyOutput([property: JsonPropertyName("links")] RelatedManyLinks Links);

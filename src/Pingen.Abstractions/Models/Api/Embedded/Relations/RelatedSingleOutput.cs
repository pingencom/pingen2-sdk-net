using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;
using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations;

/// <summary>
/// Relation to a single object
/// </summary>
public sealed record RelatedSingleOutput(
    [property: JsonPropertyName("links")] RelatedSingleLinks Links,
    [property: JsonPropertyName("data")] DataIdentity Data
);

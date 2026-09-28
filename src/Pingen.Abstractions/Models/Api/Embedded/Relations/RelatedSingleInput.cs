using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.Api.Embedded.Relations;

/// <summary>
/// Relation to a single object for input
/// </summary>
public sealed record RelatedSingleInput
{
    /// <summary>
    /// Data object or the relation
    /// </summary>
    [JsonPropertyName("data")]
    public required DataIdentity Data { get; init; }
}

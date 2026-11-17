using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// Generic data POST object with attributes based on type to send to the API
/// </summary>
public record DataPost<TAttributes> : IDataPost
    where TAttributes : IAttributes
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    /// <summary>
    /// Object data attributes based on type
    /// </summary>
    [JsonPropertyName("attributes")]
    public required TAttributes Attributes { get; init; }
}

/// <summary>
/// Generic data POST object with attributes and relationships based on type to send to the API
/// </summary>
public record DataPost<TAttributes, TRelationships> : DataPost<TAttributes>
    where TAttributes : IAttributes
    where TRelationships : IRelationships
{
    /// <summary>
    /// Relationships based on type
    /// </summary>
    [JsonPropertyName("relationships")]
    public TRelationships? Relationships { get; init; }
}

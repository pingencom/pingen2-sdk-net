using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Base;

/// <inheritdoc cref="IData" />
public record Data : DataIdentity, IData
{
    /// <inheritdoc />
    [JsonPropertyName("links")]
    public DataLinks? Links { get; init; }
}

/// <inheritdoc cref="IData{TAttributes}" />
public record Data<TAttributes> : Data, IData<TAttributes>
    where TAttributes : IAttributes
{
    /// <inheritdoc />
    [JsonPropertyName("attributes")]
    public required TAttributes Attributes { get; init; }
}

/// <inheritdoc cref="IData{TAttributes,TRelationships}" />
public record Data<TAttributes, TRelationships> : Data<TAttributes>, IData<TAttributes, TRelationships>
    where TAttributes : IAttributes
    where TRelationships : IRelationships
{
    /// <inheritdoc />
    [JsonPropertyName("relationships")]
    public required TRelationships Relationships { get; init; }
}

using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Interfaces.Data;

/// <summary>
/// Base data object interface without the actual data, but can be used to ensure an object is a data object
/// </summary>
public interface IData
{
    /// <summary>
    /// Data links object
    /// </summary>
    public DataLinks? Links { get; init; }
}

/// <summary>
/// Base data object including type based data
/// </summary>
public interface IData<TAttributes> : IData
    where TAttributes : IAttributes
{
    /// <summary>
    /// Object data attributes based on type
    /// </summary>
    public TAttributes Attributes { get; init; }
}

/// <summary>
/// Base data object including type based data and relationships
/// </summary>
public interface IData<TAttributes, TRelationships> : IData<TAttributes>
    where TAttributes : IAttributes
    where TRelationships : IRelationships
{
    /// <summary>
    /// Relationships based on type
    /// </summary>
    public TRelationships Relationships { get; init; }
}

/// <summary>
/// Base data object including type based data, relationships and meta information
/// </summary>
public interface IData<TAttributes, TRelationships, TMeta> : IData<TAttributes, TRelationships>
    where TAttributes : IAttributes
    where TRelationships : IRelationships
    where TMeta : IMeta
{
    /// <summary>
    /// Meta information based on type
    /// </summary>
    public TMeta Meta { get; init; }
}

using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Api;

/// <summary>
/// Collection sort direction
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<CollectionSortDirection>))]
public enum CollectionSortDirection
{
    /// <summary>
    /// Sort ascending
    /// </summary>
    ASC,

    /// <summary>
    /// Sort descending
    /// </summary>
    DESC,
}

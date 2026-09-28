using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Api.Embedded;

/// <summary>
/// Generic data PATCH object with attributes based on type to send to the API
/// </summary>
public sealed record DataPatch<TAttributes> : DataPost<TAttributes>, IDataPatch
    where TAttributes : IAttributes
{
    /// <inheritdoc />
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}

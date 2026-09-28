using System.Text.Json.Serialization;
using Pingen.Abstractions.Enums.Batches;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Models.Batches.Views;

/// <summary>
/// Batch edit object to send via <see cref="DataPatch{TAttributes}"/> to the API. Both fields are optional;
/// send only the ones to change.
/// </summary>
public sealed record BatchEdit : IAttributes
{
    /// <summary>
    /// Name of the batch [ 5 .. 100 ] characters.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Icon.
    /// </summary>
    [JsonPropertyName("icon")]
    public BatchIcon? Icon { get; init; }
}

using System.Text.Json.Serialization;
using Pingen.Abstractions.Models.Letters.Views;

namespace Pingen.Abstractions.Models.Letters.Embedded;

/// <summary>
/// Meta data for <see cref="LetterCreate"/>
/// </summary>
public sealed record LetterMetaData
{
    /// <summary>
    /// Recipient
    /// </summary>
    [JsonPropertyName("recipient")]
    public required LetterMetaDataContact Recipient { get; init; }

    /// <summary>
    /// Sender
    /// </summary>
    [JsonPropertyName("sender")]
    public required LetterMetaDataContact Sender { get; init; }
}

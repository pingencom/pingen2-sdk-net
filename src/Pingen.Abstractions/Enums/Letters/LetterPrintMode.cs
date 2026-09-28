using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Letters;

/// <summary>
/// Letter print mode. <see href="https://api.pingen.com/documentation#tag/letters.general/operation/letters.show">API Doc - Letter details</see>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<LetterPrintMode>))]
public enum LetterPrintMode
{
    /// <summary>
    /// Print mode simplex
    /// </summary>
    simplex,

    /// <summary>
    /// Print mode duplex
    /// </summary>
    duplex,
}

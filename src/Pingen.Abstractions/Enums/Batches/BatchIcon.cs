using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming

namespace Pingen.Abstractions.Enums.Batches;

/// <summary>
/// Batch icon
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BatchIcon>))]
public enum BatchIcon
{
    /// <summary>
    /// Campaign
    /// </summary>
    campaign,

    /// <summary>
    /// Megaphone
    /// </summary>
    megaphone,

    /// <summary>
    /// Wave-hand
    /// </summary>
    [JsonStringEnumMemberName("wave-hand")]
    waveHand,

    /// <summary>
    /// Flash
    /// </summary>
    flash,

    /// <summary>
    /// Rocket
    /// </summary>
    rocket,

    /// <summary>
    /// Bell
    /// </summary>
    bell,

    /// <summary>
    /// Percent-tag
    /// </summary>
    [JsonStringEnumMemberName("percent-tag")]
    percentTag,

    /// <summary>
    /// Percent-badge
    /// </summary>
    [JsonStringEnumMemberName("percent-badge")]
    percentBadge,

    /// <summary>
    /// Present
    /// </summary>
    present,

    /// <summary>
    /// Receipt
    /// </summary>
    receipt,

    /// <summary>
    /// Document
    /// </summary>
    document,

    /// <summary>
    /// Information
    /// </summary>
    information,

    /// <summary>
    /// Calendar
    /// </summary>
    calendar,

    /// <summary>
    /// Newspaper
    /// </summary>
    newspaper,

    /// <summary>
    /// Crown
    /// </summary>
    crown,

    /// <summary>
    /// Virus
    /// </summary>
    virus,
}

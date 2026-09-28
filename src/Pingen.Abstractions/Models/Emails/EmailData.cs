using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Emails;

/// <summary>
/// Email data object
/// </summary>
public record EmailData : Data<Email, EmailRelationships>;

/// <summary>
/// Email data object detailed
/// </summary>
public sealed record EmailDataDetailed : EmailData, IData<Email, EmailRelationships, Meta<MetaAbility<EmailAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<EmailAbilities>> Meta { get; init; }
}

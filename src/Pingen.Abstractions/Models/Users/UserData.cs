using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Base.Embedded;

namespace Pingen.Abstractions.Models.Users;

/// <summary>
/// User data object
/// </summary>
public record UserData : Data<User, UserRelationships>;

/// <summary>
/// User data object detailed
/// </summary>
public sealed record UserDataDetailed : UserData, IData<User, UserRelationships, Meta<MetaAbility<UserAbilities>>>
{
    /// <inheritdoc />
    [JsonPropertyName("meta")]
    public required Meta<MetaAbility<UserAbilities>> Meta { get; init; }
}

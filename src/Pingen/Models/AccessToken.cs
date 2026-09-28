using System.Text;
using System.Text.Json.Serialization;

namespace Pingen.Models;

/// <summary>
/// Access token
/// </summary>
internal sealed record AccessToken(
    [property: JsonPropertyName("access_token")] string Token,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] long ExpiresIn
)
{
    /// <summary>
    /// Absolute expiry, computed on construction. Uses UTC so a local-time / DST change cannot skew the
    /// token lifetime (compared against <see cref="DateTime.UtcNow"/> in the connection handler).
    /// </summary>
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddSeconds(ExpiresIn);

    /// <summary>
    /// Redacts the raw bearer token from the record's <see cref="object.ToString"/> output so it cannot leak
    /// into logs or debug dumps.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Token = ***, ");
        builder.Append($"TokenType = {TokenType}, ");
        builder.Append($"ExpiresIn = {ExpiresIn}, ");
        builder.Append($"ExpiresAt = {ExpiresAt}");

        return true;
    }
}

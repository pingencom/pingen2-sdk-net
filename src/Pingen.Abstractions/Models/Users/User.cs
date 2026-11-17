using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Models.Users;

/// <summary>
/// User
/// </summary>
/// <param name="Edition">Product edition the user is on.</param>
/// <param name="Flags">Feature flags enabled for the user.</param>
[PingenApiType("users")]
public sealed record User(
    [property: JsonPropertyName(UserFields.Email)] string? Email,
    [property: JsonPropertyName(UserFields.FirstName)] string? FirstName,
    [property: JsonPropertyName(UserFields.LastName)] string? LastName,
    [property: JsonPropertyName(UserFields.Status)] string? Status,
    [property: JsonPropertyName(UserFields.Language)] string? Language,
    [property: JsonPropertyName(UserFields.CreatedAt)] DateTime? CreatedAt,
    [property: JsonPropertyName(UserFields.UpdatedAt)] DateTime? UpdatedAt,
    [property: JsonPropertyName(UserFields.Edition)] string? Edition = null,
    [property: JsonPropertyName(UserFields.Flags)] IReadOnlyList<string>? Flags = null
) : IAttributes;

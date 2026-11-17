using System.Text;
using Pingen.Interfaces;

namespace Pingen.Services;

/// <inheritdoc />
public sealed record PingenConfiguration : IPingenConfiguration
{
    /// <summary>
    /// Redacts the OAuth client secret and webhook signing keys from the record's <see cref="object.ToString"/>
    /// output, so accidentally logging the configuration cannot leak credentials.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"BaseUri = {BaseUri}, ");
        builder.Append($"IdentityUri = {IdentityUri}, ");
        builder.Append($"ClientId = {ClientId}, ");
        builder.Append("ClientSecret = ***, ");
        builder.Append($"DefaultOrganisationId = {DefaultOrganisationId}, ");
        builder.Append(WebhookSigningKeys is null ? "WebhookSigningKeys = , " : $"WebhookSigningKeys = [{WebhookSigningKeys.Count} redacted], ");
        builder.Append($"MaxRetryAttempts = {MaxRetryAttempts}");

        return true;
    }

    /// <inheritdoc />
    public required string BaseUri { get; set; }

    /// <inheritdoc />
    public required string IdentityUri { get; set; }

    /// <inheritdoc />
    public required string ClientId { get; set; }

    /// <inheritdoc />
    public required string ClientSecret { get; set; }

    /// <inheritdoc />
    public required string DefaultOrganisationId { get; set; }

    /// <inheritdoc />
    public Dictionary<string, string>? WebhookSigningKeys { get; set; }

    /// <inheritdoc />
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Create a configuration for the given credentials, selecting the production or staging environment URLs
    /// automatically via the <paramref name="useStaging"/> switch.
    /// </summary>
    /// <param name="clientId">OAuth client id.</param>
    /// <param name="clientSecret">OAuth client secret.</param>
    /// <param name="defaultOrganisationId">Default organisation id.</param>
    /// <param name="useStaging">When <c>true</c>, target the staging environment; otherwise production (default).</param>
    /// <returns>A ready-to-use <see cref="PingenConfiguration"/>.</returns>
    public static PingenConfiguration Create(string clientId, string clientSecret, string defaultOrganisationId, bool useStaging = false) =>
        new()
        {
            BaseUri = PingenEnvironment.BaseUri(useStaging),
            IdentityUri = PingenEnvironment.IdentityUri(useStaging),
            ClientId = clientId,
            ClientSecret = clientSecret,
            DefaultOrganisationId = defaultOrganisationId,
        };
}

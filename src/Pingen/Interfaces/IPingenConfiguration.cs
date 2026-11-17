namespace Pingen.Interfaces;

/// <summary>
/// Configuration for accessing Pingen API
/// </summary>
public interface IPingenConfiguration
{
    /// <summary>
    /// Base URI for accessing the service. <see href="https://api.pingen.com/documentation#section/Basics/Environments">API Doc - Environments</see>
    /// </summary>
    public string BaseUri { get; set; }

    /// <summary>
    /// Identity URI to obtain access token. <see href="https://api.pingen.com/documentation#section/Basics/Environments">API Doc - Environments</see>
    /// </summary>
    public string IdentityUri { get; set; }

    /// <summary>
    /// Generated client id. <see href="https://api.pingen.com/documentation#section/Authentication">API Doc - Authentication</see>
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Generated client secret. <see href="https://api.pingen.com/documentation#section/Authentication">API Doc - Authentication</see>
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Default organisation ID to use. Can be changed later using <see cref="IPingenApiClient.SetOrganisationId"/>. <see href="https://api.pingen.com/documentation#section/Quickstart">API Doc - Quickstart</see>
    /// </summary>
    public string DefaultOrganisationId { get; set; }

    /// <summary>
    /// Dictionary with webhook guid and the assigned signing key.
    /// The signing key is also stated as secret sometimes in the pingen documentation.
    /// </summary>
    public Dictionary<string, string>? WebhookSigningKeys { get; set; }

    /// <summary>
    /// Maximum number of attempts for a single API request (initial attempt + retries) when the API answers
    /// with a retryable status. <c>429</c> is always retried; <c>5xx</c> is retried for idempotent methods
    /// (GET/PUT/DELETE) and for POST/PATCH only when an idempotency key is supplied. Between attempts an
    /// exponential backoff with jitter is applied, honouring the <c>Retry-After</c> header. A one-time token
    /// refresh on <c>401</c> is handled separately and does not consume this budget.
    /// Set to <c>1</c> to disable retries. Defaults to <c>3</c>.
    /// </summary>
    public int MaxRetryAttempts { get; set; }
}

/// <summary>
/// Extension methods for <see cref="IPingenConfiguration"/>.
/// </summary>
public static class PingenConfigurationExtension
{
    /// <summary>
    /// Normalize configuration.
    /// </summary>
    public static IPingenConfiguration Normalize(this IPingenConfiguration configuration)
    {
        if (!Uri.TryCreate(configuration.BaseUri, UriKind.Absolute, out var baseUri) || baseUri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("BaseUri must use the HTTPS scheme.", nameof(configuration));

        if (!Uri.TryCreate(configuration.IdentityUri, UriKind.Absolute, out var identityUri) || identityUri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("IdentityUri must use the HTTPS scheme.", nameof(configuration));

        if (!configuration.BaseUri.EndsWith('/'))
            configuration.BaseUri += '/';

        if (!configuration.IdentityUri.EndsWith('/'))
            configuration.IdentityUri += '/';

        return configuration;
    }
}

namespace Pingen.Services;

/// <summary>
/// Well-known Pingen environment base URLs, matching the <c>useStaging</c> switch used by the other Pingen
/// SDKs. <see href="https://api.pingen.com/documentation#section/Basics/Environments">API Doc - Environments</see>
/// </summary>
public static class PingenEnvironment
{
    /// <summary>Production API base URI.</summary>
    public const string ProductionBaseUri = "https://api.pingen.com";

    /// <summary>Production identity (OAuth token) base URI.</summary>
    public const string ProductionIdentityUri = "https://identity.pingen.com";

    /// <summary>Staging API base URI.</summary>
    public const string StagingBaseUri = "https://api-staging.pingen.com";

    /// <summary>Staging identity (OAuth token) base URI.</summary>
    public const string StagingIdentityUri = "https://identity-staging.pingen.com";

    /// <summary>API base URI for the selected environment.</summary>
    public static string BaseUri(bool useStaging) => useStaging ? StagingBaseUri : ProductionBaseUri;

    /// <summary>Identity base URI for the selected environment.</summary>
    public static string IdentityUri(bool useStaging) => useStaging ? StagingIdentityUri : ProductionIdentityUri;
}

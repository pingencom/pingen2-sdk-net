using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Abstractions.Exceptions;

/// <summary>
/// Represents a failure returned by the Pingen OAuth token endpoint while acquiring an access token.
/// </summary>
public class PingenApiAuthErrorException : PingenException
{
    /// <summary>
    /// The parsed OAuth error, when the response body could be deserialized; otherwise <c>null</c>.
    /// </summary>
    public PingenApiAuthError? AuthError { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenApiAuthErrorException"/> class.
    /// </summary>
    public PingenApiAuthErrorException(PingenApiAuthError? authError, string message)
        : base(message)
    {
        AuthError = authError;
    }
}

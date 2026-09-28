namespace Pingen.Abstractions.Exceptions;

/// <summary>
/// Represents errors that occur from pingen file download
/// </summary>
public class PingenFileDownloadException : PingenException
{
    /// <summary>
    /// Error code
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenFileDownloadException"/> class, deriving a
    /// human-readable message from the error code so <see cref="System.Exception.Message"/> is useful in logs.
    /// </summary>
    public PingenFileDownloadException(string? errorCode)
        : base(string.IsNullOrEmpty(errorCode) ? "Pingen file download failed." : $"Pingen file download failed: {errorCode}.")
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenFileDownloadException"/> class
    /// </summary>
    public PingenFileDownloadException(string? errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenFileDownloadException"/> class
    /// </summary>
    public PingenFileDownloadException(string? errorCode, string message, Exception inner)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }
}

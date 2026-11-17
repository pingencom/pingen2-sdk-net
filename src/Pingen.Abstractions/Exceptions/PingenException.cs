namespace Pingen.Abstractions.Exceptions;

/// <summary>
/// Base type for every exception thrown by the Pingen SDK, so consumers can catch all SDK errors with a
/// single <c>catch (PingenException)</c>. Derives from <see cref="Exception"/> rather than the discouraged
/// <see cref="ApplicationException"/>.
/// </summary>
public abstract class PingenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PingenException"/> class.
    /// </summary>
    protected PingenException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenException"/> class.
    /// </summary>
    protected PingenException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenException"/> class.
    /// </summary>
    protected PingenException(string message, Exception inner)
        : base(message, inner) { }
}

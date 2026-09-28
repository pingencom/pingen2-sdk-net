using Pingen.Abstractions.Models.Api;

namespace Pingen.Abstractions.Exceptions;

/// <summary>
/// Represents errors that occur from Pingen API responses
/// </summary>
public class PingenApiErrorException : PingenException
{
    /// <summary>
    /// API Result
    /// </summary>
    public ApiResult? ApiResult { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenApiErrorException"/> class, deriving a human-readable
    /// message from the API error payload so <see cref="System.Exception.Message"/> is useful in logs.
    /// </summary>
    public PingenApiErrorException(ApiResult apiResult)
        : base(BuildMessage(apiResult))
    {
        ApiResult = apiResult;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenApiErrorException"/> class
    /// </summary>
    public PingenApiErrorException(ApiResult apiResult, string message)
        : base(message)
    {
        ApiResult = apiResult;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenApiErrorException"/> class
    /// </summary>
    public PingenApiErrorException(ApiResult apiResult, string message, Exception inner)
        : base(message, inner)
    {
        ApiResult = apiResult;
    }

    private static string BuildMessage(ApiResult apiResult)
    {
        var error = apiResult.ApiError?.Errors.FirstOrDefault();

        return error is null ? "The Pingen API request failed." : $"The Pingen API request failed: {error.Code} - {error.Detail}";
    }
}

using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;

namespace Pingen.Tests.Exceptions;

public class PingenExceptionTests
{
    [Test]
    public void PingenApiErrorException_WithApiResult_StoresApiResult()
    {
        var apiResult = new ApiResult { IsSuccess = false, RequestId = Guid.NewGuid() };

        var exception = new PingenApiErrorException(apiResult);

        exception.ApiResult.ShouldBeSameAs(apiResult);
    }

    [Test]
    public void PingenApiErrorException_WithApiError_DerivesMessageFromError()
    {
        var apiResult = new ApiResult
        {
            IsSuccess = false,
            ApiError = new ApiError([new ApiErrorData("unprocessable_entity", "Error", "Country code is invalid", new ApiErrorSource("", ""))]),
        };

        var exception = new PingenApiErrorException(apiResult);

        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldContain("unprocessable_entity"),
            () => exception.Message.ShouldContain("Country code is invalid")
        );
    }

    [Test]
    public void PingenApiErrorException_WithoutApiError_UsesGenericMessage()
    {
        var exception = new PingenApiErrorException(new ApiResult { IsSuccess = false });

        exception.Message.ShouldBe("The Pingen API request failed.");
    }

    [Test]
    public void PingenFileDownloadException_WithErrorCode_DerivesMessage()
    {
        var exception = new PingenFileDownloadException("AccessDenied");

        exception.Message.ShouldBe("Pingen file download failed: AccessDenied.");
    }

    [Test]
    public void PingenFileDownloadException_NullErrorCode_UsesGenericMessage()
    {
        var exception = new PingenFileDownloadException((string?)null);

        exception.Message.ShouldBe("Pingen file download failed.");
    }

    [Test]
    public void AllSdkExceptions_ShareThePingenExceptionBase()
    {
        PingenException[] exceptions =
        [
            new PingenApiErrorException(new ApiResult { IsSuccess = false }),
            new PingenFileDownloadException("code"),
            new PingenApiAuthErrorException(null, "auth failed"),
            new PingenWebhookValidationErrorException(null),
        ];

        exceptions.ShouldAllBe(e => e is PingenException);
    }

    [Test]
    public void PingenApiErrorException_WithMessage_StoresMessage()
    {
        var apiResult = new ApiResult { IsSuccess = false };
        const string message = "Test error message";

        var exception = new PingenApiErrorException(apiResult, message);

        exception.ShouldSatisfyAllConditions(() => exception.Message.ShouldBe(message), () => exception.ApiResult.ShouldBeSameAs(apiResult));
    }

    [Test]
    public void PingenApiErrorException_WithInnerException_StoresInnerException()
    {
        var apiResult = new ApiResult { IsSuccess = false };
        var inner = new InvalidOperationException("inner");

        var exception = new PingenApiErrorException(apiResult, "msg", inner);

        exception.ShouldSatisfyAllConditions(() => exception.InnerException.ShouldBeSameAs(inner), () => exception.ApiResult.ShouldBeSameAs(apiResult));
    }

    [Test]
    public void PingenApiAuthErrorException_WithAuthError_StoresErrorAndMessage()
    {
        var authError = new PingenApiAuthError("invalid_client", "Client authentication failed", "Unauthorized");

        var exception = new PingenApiAuthErrorException(authError, "Client authentication failed");

        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Client authentication failed"),
            () => exception.AuthError.ShouldBeSameAs(authError)
        );
    }

    [Test]
    public void PingenApiAuthErrorException_NullAuthError_Allowed()
    {
        var exception = new PingenApiAuthErrorException(null, "Failed to obtain access token.");

        exception.ShouldSatisfyAllConditions(() => exception.AuthError.ShouldBeNull(), () => exception.Message.ShouldBe("Failed to obtain access token."));
    }

    [Test]
    public void PingenFileDownloadException_WithErrorCode_StoresErrorCode()
    {
        const string errorCode = "AccessDenied";

        var exception = new PingenFileDownloadException(errorCode);

        exception.ErrorCode.ShouldBe(errorCode);
    }

    [Test]
    public void PingenFileDownloadException_WithMessage_StoresMessageAndCode()
    {
        const string errorCode = "NoSuchKey";
        const string message = "File not found";

        var exception = new PingenFileDownloadException(errorCode, message);

        exception.ShouldSatisfyAllConditions(() => exception.ErrorCode.ShouldBe(errorCode), () => exception.Message.ShouldBe(message));
    }

    [Test]
    public void PingenFileDownloadException_WithInnerException_StoresAll()
    {
        var inner = new Exception("inner");

        var exception = new PingenFileDownloadException("err", "msg", inner);

        exception.InnerException.ShouldBeSameAs(inner);
    }

    [Test]
    public void PingenFileDownloadException_NullErrorCode_Allowed()
    {
        var exception = new PingenFileDownloadException(null);

        exception.ErrorCode.ShouldBeNull();
    }

    [Test]
    public void PingenWebhookValidationErrorException_WithNullData_Allowed()
    {
        var exception = new PingenWebhookValidationErrorException(null);

        exception.WebhookEventData.ShouldBeNull();
    }

    [Test]
    public void PingenWebhookValidationErrorException_WithMessage_StoresMessage()
    {
        const string message = "Validation failed";

        var exception = new PingenWebhookValidationErrorException(null, message);

        exception.Message.ShouldBe(message);
    }

    [Test]
    public void PingenWebhookValidationErrorException_WithInnerException_StoresAll()
    {
        var inner = new Exception("inner");

        var exception = new PingenWebhookValidationErrorException(null, "msg", inner);

        exception.InnerException.ShouldBeSameAs(inner);
    }
}

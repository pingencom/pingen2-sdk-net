using System.Net;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api;

namespace Pingen.Tests.Models;

public class ExternalRequestResultTests
{
    [Test]
    public void ExternalRequestResult_DefaultValues_AreCorrect()
    {
        var result = new ExternalRequestResult();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.StatusCode.ShouldBe(default),
            () => result.ReasonPhrase.ShouldBeNull()
        );
    }

    [Test]
    public void ExternalRequestResult_WithValues_StoresCorrectly()
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
        };

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.StatusCode.ShouldBe(HttpStatusCode.OK),
            () => result.ReasonPhrase.ShouldBe("OK")
        );
    }

    [Test]
    public void ExternalRequestResult_RecordEquality_Works()
    {
        var result1 = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
        };

        var result2 = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
        };

        result1.ShouldBe(result2);
    }

    [Test]
    public void ExternalRequestResult_DifferentStatusCodes_AreNotEqual()
    {
        var success = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
        };

        var failure = new ExternalRequestResult
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.Forbidden,
            ReasonPhrase = "Forbidden",
        };

        success.ShouldNotBe(failure);
    }

    [Test]
    public void ExternalRequestResult_ErrorStatusCodes_StoredCorrectly()
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.ServiceUnavailable,
            ReasonPhrase = "Service Unavailable",
        };

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable),
            () => result.ReasonPhrase.ShouldBe("Service Unavailable")
        );
    }

    [Test]
    public void ExternalRequestResult_NullReasonPhrase_StoredAsNull()
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = null,
        };

        result.ReasonPhrase.ShouldBeNull();
    }

    [Test]
    public void ExternalRequestResult_RecordEquality_DifferingReasonPhrase_NotEqual()
    {
        var a = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
        };

        var b = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "Okay",
        };

        a.ShouldNotBe(b);
    }

    [Test]
    public void ExternalRequestResult_Serialization_RoundTrip_PreservesAllFields()
    {
        var original = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.Created,
            ReasonPhrase = "Created",
        };

        string json = PingenSerialisationHelper.Serialize(original);
        ExternalRequestResult roundTripped = PingenSerialisationHelper.Deserialize<ExternalRequestResult>(json)!;

        roundTripped.ShouldBe(original);
    }

    [Test]
    public void ExternalRequestResult_Serialization_NullReasonPhrase_OmittedFromJson()
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.NoContent,
            ReasonPhrase = null,
        };

        string json = PingenSerialisationHelper.Serialize(result);

        json.ShouldNotContain("ReasonPhrase");
    }

    [TestCase(HttpStatusCode.OK)]
    [TestCase(HttpStatusCode.Created)]
    [TestCase(HttpStatusCode.NoContent)]
    public void ExternalRequestResult_StatusCodeCategory_2xx_IsSuccessTrue(HttpStatusCode statusCode)
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = true,
            StatusCode = statusCode,
            ReasonPhrase = statusCode.ToString(),
        };

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.StatusCode.ShouldBe(statusCode));
    }

    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.InternalServerError)]
    [TestCase(HttpStatusCode.BadGateway)]
    [TestCase(HttpStatusCode.ServiceUnavailable)]
    public void ExternalRequestResult_StatusCodeCategory_NonSuccess_IsSuccessFalse(HttpStatusCode statusCode)
    {
        var result = new ExternalRequestResult
        {
            IsSuccess = false,
            StatusCode = statusCode,
            ReasonPhrase = statusCode.ToString(),
        };

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.StatusCode.ShouldBe(statusCode));
    }
}

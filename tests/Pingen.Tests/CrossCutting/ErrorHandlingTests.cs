using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Letters;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class ErrorHandlingTests : EndpointTestBase
{
    [TestCase(401)]
    [TestCase(403)]
    [TestCase(404)]
    [TestCase(422)]
    [TestCase(500)]
    [TestCase(502)]
    [TestCase(503)]
    [TestCase(504)]
    public async Task GetPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException(int statusCode)
    {
        Server.StubError(
            OrgPath("deliveries/letters"),
            "GET",
            PingenResponseFactory.ErrorResponse($"Error {statusCode}", $"HTTP {statusCode} returned by stub", statusCode.ToString()),
            statusCode
        );

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<LetterData> _ in Client.Letters.GetPageResultsAsync())
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task GetPageResultsAsync_OnApiError_ShouldParseJsonApiErrorEnvelope()
    {
        Server.StubError(OrgPath("deliveries/letters"), "GET", PingenResponseFactory.ErrorResponse("Validation failed", "The request payload is invalid"));

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<LetterData> _ in Client.Letters.GetPageResultsAsync())
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.ApiError.ShouldNotBeNull();
        exception.ApiResult.ApiError!.Errors.Count.ShouldBeGreaterThan(0);
        exception.ApiResult.ApiError.Errors[0].Title.ShouldBe("Validation failed");
        exception.ApiResult.ApiError.Errors[0].Detail.ShouldBe("The request payload is invalid");
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(404)]
    [TestCase(422)]
    [TestCase(500)]
    [TestCase(502)]
    [TestCase(503)]
    [TestCase(504)]
    public async Task GetPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError(OrgPath("batches"), "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetPage_On429TooManyRequests_ShouldExposeRetryAfter()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath("batches")).UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(429)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Retry-After", "30")
                    .WithBody(PingenResponseFactory.ErrorResponse("Too Many Requests", "Rate limit exceeded", "429"))
            );

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.RetryAfter.ShouldBe(30),
            () => result.ApiError.ShouldNotBeNull()
        );
    }
}

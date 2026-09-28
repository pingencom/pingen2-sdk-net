using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Api.Embedded.DataResults.Embedded;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Letters;

namespace Pingen.Tests.Models;

public class ApiResultTests
{
    [Test]
    public void ApiResult_DefaultValues_AreCorrect()
    {
        var result = new ApiResult();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.RequestId.ShouldBe(Guid.Empty),
            () => result.RateLimitLimit.ShouldBe(0),
            () => result.RateLimitRemaining.ShouldBe(0),
            () => result.RateLimitReset.ShouldBeNull(),
            () => result.RetryAfter.ShouldBeNull(),
            () => result.IdempotentReplayed.ShouldBeFalse(),
            () => result.ApiError.ShouldBeNull(),
            () => result.Location.ShouldBeNull()
        );
    }

    [Test]
    public void ApiResult_WithValues_StoresCorrectly()
    {
        var requestId = Guid.NewGuid();
        DateTimeOffset resetTime = DateTimeOffset.UtcNow;
        var location = new Uri("https://example.com/redirect");

        var result = new ApiResult
        {
            IsSuccess = true,
            RequestId = requestId,
            RateLimitLimit = 100,
            RateLimitRemaining = 99,
            RateLimitReset = resetTime,
            RetryAfter = 30,
            IdempotentReplayed = true,
            Location = location,
        };

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.RequestId.ShouldBe(requestId),
            () => result.RateLimitLimit.ShouldBe(100),
            () => result.RateLimitRemaining.ShouldBe(99),
            () => result.RateLimitReset.ShouldBe(resetTime),
            () => result.RetryAfter.ShouldBe(30),
            () => result.IdempotentReplayed.ShouldBeTrue(),
            () => result.Location.ShouldBe(location)
        );
    }

    [Test]
    public void ApiResultGeneric_WithData_StoresData()
    {
        var result = new ApiResult<SingleResult<LetterData>> { IsSuccess = true, Data = null };

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data.ShouldBeNull());
    }

    [Test]
    public void ApiResultGeneric_InheritsBaseProperties()
    {
        var requestId = Guid.NewGuid();

        var result = new ApiResult<CollectionResult<LetterData>>
        {
            IsSuccess = true,
            RequestId = requestId,
            RateLimitLimit = 50,
            Data = null,
        };

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.RequestId.ShouldBe(requestId),
            () => result.RateLimitLimit.ShouldBe(50)
        );
    }

    [Test]
    public void ApiResult_WithApiError_StoresError()
    {
        var error = new ApiError([new ApiErrorData("422", "Validation Error", "Field is invalid", new ApiErrorSource("/data/attributes/name", ""))]);

        var result = new ApiResult { IsSuccess = false, ApiError = error };

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBe(1)
        );
    }

    [Test]
    public void CollectionResultMeta_StoresPaginationValues()
    {
        var meta = new CollectionResultMeta(1, 5, 20, 1, 20, 100);

        meta.ShouldSatisfyAllConditions(
            () => meta.CurrentPage.ShouldBe(1),
            () => meta.LastPage.ShouldBe(5),
            () => meta.PerPage.ShouldBe(20),
            () => meta.From.ShouldBe(1),
            () => meta.To.ShouldBe(20),
            () => meta.Total.ShouldBe(100)
        );
    }

    [Test]
    public void ApiResultGeneric_DefaultData_IsNull()
    {
        var result = new ApiResult<SingleResult<Data<Letter>>>();

        result.ShouldSatisfyAllConditions(() => result.Data.ShouldBeNull(), () => result.IsSuccess.ShouldBeFalse());
    }

    [Test]
    public void ApiResultGeneric_RecordEquality_SameValues_AreEqual()
    {
        var requestId = Guid.NewGuid();
        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        var r1 = new ApiResult<SingleResult<Data<Letter>>>
        {
            IsSuccess = true,
            RequestId = requestId,
            RateLimitLimit = 100,
            RateLimitRemaining = 99,
            RateLimitReset = resetTime,
            Data = null,
        };

        var r2 = new ApiResult<SingleResult<Data<Letter>>>
        {
            IsSuccess = true,
            RequestId = requestId,
            RateLimitLimit = 100,
            RateLimitRemaining = 99,
            RateLimitReset = resetTime,
            Data = null,
        };

        r1.ShouldBe(r2);
    }

    [Test]
    public void ApiResultGeneric_RecordEquality_DifferentRequestId_AreNotEqual()
    {
        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        var r1 = new ApiResult<SingleResult<Data<Letter>>>
        {
            IsSuccess = true,
            RequestId = Guid.NewGuid(),
            RateLimitReset = resetTime,
            Data = null,
        };

        var r2 = new ApiResult<SingleResult<Data<Letter>>>
        {
            IsSuccess = true,
            RequestId = Guid.NewGuid(),
            RateLimitReset = resetTime,
            Data = null,
        };

        r1.ShouldNotBe(r2);
    }

    [Test]
    public void Deserialize_SingleResult_ProducesPopulatedDataAndIncludedNull()
    {
        string json = """
            {
                "data": {
                    "id": "letter-1",
                    "type": "letters",
                    "attributes": { "status": "valid" }
                }
            }
            """;

        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.ShouldSatisfyAllConditions(
            () => result.Data.Id.ShouldBe("letter-1"),
            () => result.Data.Attributes.Status.ShouldBe("valid"),
            () => result.Included.ShouldBeNull()
        );
    }

    [Test]
    public void Deserialize_SingleResult_WithEmptyIncluded_ProducesEmptyCollection()
    {
        string json = """
            {
                "data": {
                    "id": "letter-1",
                    "type": "letters",
                    "attributes": { "status": "valid" }
                },
                "included": []
            }
            """;

        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.ShouldSatisfyAllConditions(
            () => result.Included.ShouldNotBeNull(),
            () => result.Included!.Count.ShouldBe(0),
            () => result.Included!.RawItems.ShouldBeEmpty()
        );
    }

    [Test]
    public void Deserialize_CollectionResult_ProducesDataAndMeta()
    {
        string json = """
            {
                "data": [
                    { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
                ],
                "links": {
                    "first": "https://example.com/?page=1",
                    "last": "https://example.com/?page=2",
                    "prev": "",
                    "next": "https://example.com/?page=2",
                    "self": "https://example.com/?page=1"
                },
                "meta": {
                    "current_page": 1,
                    "last_page": 2,
                    "per_page": 10,
                    "from": 1,
                    "to": 10,
                    "total": 15
                }
            }
            """;

        CollectionResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<CollectionResult<Data<Letter>>>(json)!;

        result.ShouldSatisfyAllConditions(
            () => result.Data.Count.ShouldBe(1),
            () => result.Meta.CurrentPage.ShouldBe(1),
            () => result.Meta.LastPage.ShouldBe(2),
            () => result.Meta.Total.ShouldBe(15),
            () => result.Links.Self.ShouldBe("https://example.com/?page=1"),
            () => result.Included.ShouldBeNull()
        );
    }

    [Test]
    public void Deserialize_CollectionResult_WithEmptyDataArray_ReturnsEmptyList()
    {
        string json = """
            {
                "data": [],
                "links": {
                    "first": "https://example.com/?page=1",
                    "last": "https://example.com/?page=1",
                    "prev": "",
                    "next": "",
                    "self": "https://example.com/?page=1"
                },
                "meta": {
                    "current_page": 1,
                    "last_page": 1,
                    "per_page": 10,
                    "from": null,
                    "to": null,
                    "total": 0
                }
            }
            """;

        CollectionResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<CollectionResult<Data<Letter>>>(json)!;

        result.Data.ShouldBeEmpty();
    }

    [Test]
    public void Deserialize_ApiErrorEnvelope_ProducesParsedErrors()
    {
        string json = """
            {
                "errors": [
                    {
                        "code": "422",
                        "title": "Validation Error",
                        "detail": "Field is required",
                        "source": { "pointer": "/data/attributes/file_url", "parameter": "" }
                    }
                ]
            }
            """;

        ApiError error = PingenSerialisationHelper.Deserialize<ApiError>(json)!;

        error.ShouldSatisfyAllConditions(
            () => error.Errors.Count.ShouldBe(1),
            () => error.Errors[0].Code.ShouldBe("422"),
            () => error.Errors[0].Title.ShouldBe("Validation Error"),
            () => error.Errors[0].Detail.ShouldBe("Field is required"),
            () => error.Errors[0].Source.Pointer.ShouldBe("/data/attributes/file_url"),
            () => error.Errors[0].Source.Parameter.ShouldBe("")
        );
    }

    [Test]
    public void Deserialize_ApiErrorEnvelope_WithMultipleErrors_ParsesAll()
    {
        string json = """
            {
                "errors": [
                    {
                        "code": "422",
                        "title": "Validation Error",
                        "detail": "Field is required",
                        "source": { "pointer": "/data/attributes/file_url", "parameter": "" }
                    },
                    {
                        "code": "401",
                        "title": "Unauthorized",
                        "detail": "Token expired",
                        "source": { "pointer": "", "parameter": "access_token" }
                    }
                ]
            }
            """;

        ApiError error = PingenSerialisationHelper.Deserialize<ApiError>(json)!;

        error.ShouldSatisfyAllConditions(
            () => error.Errors.Count.ShouldBe(2),
            () => error.Errors[0].Code.ShouldBe("422"),
            () => error.Errors[0].Source.Pointer.ShouldBe("/data/attributes/file_url"),
            () => error.Errors[1].Code.ShouldBe("401"),
            () => error.Errors[1].Title.ShouldBe("Unauthorized"),
            () => error.Errors[1].Source.Parameter.ShouldBe("access_token")
        );
    }

    [Test]
    public void Serialize_ApiError_RoundTrip_PreservesAllFields()
    {
        var original = new ApiError([
            new ApiErrorData("422", "Validation Error", "Field is required", new ApiErrorSource("/data/attributes/file_url", "page")),
        ]);

        string json = PingenSerialisationHelper.Serialize(original);
        ApiError roundTripped = PingenSerialisationHelper.Deserialize<ApiError>(json)!;

        roundTripped.ShouldSatisfyAllConditions(
            () => roundTripped.Errors.Count.ShouldBe(1),
            () => roundTripped.Errors[0].Code.ShouldBe("422"),
            () => roundTripped.Errors[0].Title.ShouldBe("Validation Error"),
            () => roundTripped.Errors[0].Detail.ShouldBe("Field is required"),
            () => roundTripped.Errors[0].Source.Pointer.ShouldBe("/data/attributes/file_url"),
            () => roundTripped.Errors[0].Source.Parameter.ShouldBe("page")
        );
    }

    [Test]
    public void CollectionResultMeta_AllNullableValues_NullByDefault()
    {
        var meta = new CollectionResultMeta(null, null, null, null, null, null);

        meta.ShouldSatisfyAllConditions(
            () => meta.CurrentPage.ShouldBeNull(),
            () => meta.LastPage.ShouldBeNull(),
            () => meta.PerPage.ShouldBeNull(),
            () => meta.From.ShouldBeNull(),
            () => meta.To.ShouldBeNull(),
            () => meta.Total.ShouldBeNull()
        );
    }
}

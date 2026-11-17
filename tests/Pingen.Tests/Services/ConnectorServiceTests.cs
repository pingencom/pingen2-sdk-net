using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Api.Embedded.DataResults.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.Relations;
using Pingen.Abstractions.Models.Api.Embedded.Relations.Embedded;
using Pingen.Abstractions.Models.Base.Embedded;
using Pingen.Abstractions.Models.Letters;
using Pingen.Services.Connectors;

namespace Pingen.Tests.Services;

public class ConnectorServiceTests
{
    private IPingenConnectionHandler _mockConnectionHandler = null!;
    private LetterService _letterService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConnectionHandler = Substitute.For<IPingenConnectionHandler>();
        _letterService = new LetterService(_mockConnectionHandler);
    }

    [Test]
    public void HandleResult_CollectionResult_Success_ReturnsData()
    {
        var letterData = CreateLetterData("letter-1");
        var collectionResult = new CollectionResult<LetterData>(
            [letterData],
            new CollectionResultLinks("", "", "", "", ""),
            new CollectionResultMeta(1, 1, 20, 1, 1, 1)
        );

        var apiResult = new ApiResult<CollectionResult<LetterData>> { IsSuccess = true, Data = collectionResult };

        var result = _letterService.HandleResult(apiResult);

        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe("letter-1");
    }

    [Test]
    public void HandleResult_CollectionResult_Failure_ThrowsPingenApiErrorException()
    {
        var apiResult = new ApiResult<CollectionResult<LetterData>> { IsSuccess = false };

        var ex = Should.Throw<PingenApiErrorException>(() => _letterService.HandleResult(apiResult));
        ex.ApiResult.ShouldNotBeNull();
    }

    [Test]
    public void HandleResult_SingleResult_Success_ReturnsData()
    {
        var letterData = CreateLetterDataDetailed("letter-2");
        var singleResult = new SingleResult<LetterDataDetailed>(letterData);

        var apiResult = new ApiResult<SingleResult<LetterDataDetailed>> { IsSuccess = true, Data = singleResult };

        var result = _letterService.HandleResult(apiResult);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("letter-2");
    }

    [Test]
    public void HandleResult_SingleResult_Failure_ThrowsPingenApiErrorException()
    {
        var apiResult = new ApiResult<SingleResult<LetterDataDetailed>> { IsSuccess = false };

        Should.Throw<PingenApiErrorException>(() => _letterService.HandleResult(apiResult));
    }

    [Test]
    public void HandleResult_CollectionResult_NullData_ReturnsEmptyList()
    {
        var apiResult = new ApiResult<CollectionResult<LetterData>> { IsSuccess = true, Data = null };

        var result = _letterService.HandleResult(apiResult);

        result.ShouldBeEmpty();
    }

    [Test]
    public void HandleResult_SingleResult_NullDataOnSuccess_ThrowsPingenApiErrorException()
    {
        var apiResult = new ApiResult<SingleResult<LetterDataDetailed>> { IsSuccess = true, Data = null };

        Should.Throw<PingenApiErrorException>(() => _letterService.HandleResult(apiResult));
    }

    [Test]
    public async Task AutoPage_PreservesAllRequestProperties()
    {
        var sorting = new[] { new KeyValuePair<string, CollectionSortDirection>("status", CollectionSortDirection.ASC) };
        var filtering = new KeyValuePair<string, object>("and", new[] { new KeyValuePair<string, string>("status", "draft") });
        var sparseFieldsets = new[] { new KeyValuePair<string, IEnumerable<string>>("letters", ["status"]) };
        var includes = new[] { "organisation" };

        var originalRequest = new ApiPagingRequest
        {
            Sorting = sorting,
            Filtering = filtering,
            Searching = "test-search",
            PageNumber = 3,
            PageLimit = 50,
            SparseFieldsets = sparseFieldsets,
            Include = includes,
        };

        ApiPagingRequest? capturedRequest = null;

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Do<ApiPagingRequest?>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData("letter-1")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(1, 1, 50, 1, 1, 1)
                        ),
                    }
                )
            );

        await foreach (var _ in _letterService.GetPageResultsAsync(originalRequest))
        {
            // consume the page
        }

        capturedRequest.ShouldNotBeNull();
        capturedRequest.Sorting.ShouldBe(sorting);
        capturedRequest.Filtering.ShouldBe(filtering);
        capturedRequest.Searching.ShouldBe("test-search");
        capturedRequest.PageNumber.ShouldBe(3);
        capturedRequest.PageLimit.ShouldBe(50);
        capturedRequest.SparseFieldsets.ShouldBe(sparseFieldsets);
        capturedRequest.Include.ShouldBe(includes);
    }

    [Test]
    public async Task AutoPage_NullRequest_DefaultsPageNumberToOne()
    {
        ApiPagingRequest? capturedRequest = null;

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Do<ApiPagingRequest?>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData("letter-1")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(1, 1, 20, 1, 1, 1)
                        ),
                    }
                )
            );

        await foreach (var _ in _letterService.GetPageResultsAsync(null))
        {
            // consume the page
        }

        capturedRequest.ShouldNotBeNull();
        capturedRequest.PageNumber.ShouldBe(1);
    }

    [Test]
    public async Task AutoPage_NullPageNumber_DefaultsToOne()
    {
        var originalRequest = new ApiPagingRequest
        {
            Searching = "test",
            PageNumber = null,
            PageLimit = 25,
        };

        ApiPagingRequest? capturedRequest = null;

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Do<ApiPagingRequest?>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData("letter-1")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(1, 1, 25, 1, 1, 1)
                        ),
                    }
                )
            );

        await foreach (var _ in _letterService.GetPageResultsAsync(originalRequest))
        {
            // consume the page
        }

        capturedRequest.ShouldNotBeNull();
        capturedRequest.PageNumber.ShouldBe(1);
        capturedRequest.Searching.ShouldBe("test");
        capturedRequest.PageLimit.ShouldBe(25);
    }

    [Test]
    public async Task AutoPage_EmptyResultSet_YieldsSingleEmptyCollection()
    {
        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>([], new CollectionResultLinks("", "", "", "", ""), new CollectionResultMeta(1, 1, 20, 0, 0, 0)),
                    }
                )
            );

        var pages = new List<IEnumerable<LetterData>>();
        await foreach (var page in _letterService.GetPageResultsAsync())
        {
            pages.Add(page);
        }

        pages.Count.ShouldBe(1);
        pages[0].ShouldBeEmpty();
    }

    [Test]
    public async Task AutoPage_LastPageZero_StopsAfterFirstPage()
    {
        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData("letter-1")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(1, 0, 20, 1, 1, 1)
                        ),
                    }
                )
            );

        var pages = new List<IEnumerable<LetterData>>();
        await foreach (var page in _letterService.GetPageResultsAsync())
        {
            pages.Add(page);
        }

        pages.Count.ShouldBe(1);
        await _mockConnectionHandler
            .Received(1)
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AutoPage_MultiplePages_IteratesAllPages()
    {
        var callCount = 0;
        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                return Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData($"letter-{callCount}")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(callCount, 3, 1, callCount, callCount, 3)
                        ),
                    }
                );
            });

        var pages = new List<IEnumerable<LetterData>>();
        await foreach (var page in _letterService.GetPageResultsAsync())
        {
            pages.Add(page);
        }

        pages.Count.ShouldBe(3);
        pages[0].First().Id.ShouldBe("letter-1");
        pages[1].First().Id.ShouldBe("letter-2");
        pages[2].First().Id.ShouldBe("letter-3");
    }

    [Test]
    public async Task AutoPage_MultiplePages_IncrementsPageNumber()
    {
        var capturedRequests = new List<ApiPagingRequest?>();
        var callCount = 0;

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Do<ApiPagingRequest?>(r => capturedRequests.Add(r)), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                return Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData($"letter-{callCount}")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(callCount, 3, 1, callCount, callCount, 3)
                        ),
                    }
                );
            });

        await foreach (var _ in _letterService.GetPageResultsAsync())
        {
            // consume pages
        }

        capturedRequests.Count.ShouldBe(3);
        capturedRequests[0]!.PageNumber.ShouldBe(1);
        capturedRequests[1]!.PageNumber.ShouldBe(2);
        capturedRequests[2]!.PageNumber.ShouldBe(3);
    }

    [Test]
    public async Task AutoPage_MultiplePages_PreservesFilterAndSort()
    {
        var sorting = new[] { new KeyValuePair<string, CollectionSortDirection>("status", CollectionSortDirection.ASC) };
        var filtering = new KeyValuePair<string, object>("and", new[] { new KeyValuePair<string, string>("status", "draft") });

        var capturedRequests = new List<ApiPagingRequest?>();
        var callCount = 0;

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Do<ApiPagingRequest?>(r => capturedRequests.Add(r)), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                return Task.FromResult(
                    new ApiResult<CollectionResult<LetterData>>
                    {
                        IsSuccess = true,
                        Data = new CollectionResult<LetterData>(
                            [CreateLetterData($"letter-{callCount}")],
                            new CollectionResultLinks("", "", "", "", ""),
                            new CollectionResultMeta(callCount, 2, 1, callCount, callCount, 2)
                        ),
                    }
                );
            });

        var request = new ApiPagingRequest { Sorting = sorting, Filtering = filtering };

        await foreach (var _ in _letterService.GetPageResultsAsync(request))
        {
            // consume pages
        }

        capturedRequests.Count.ShouldBe(2);
        capturedRequests[0]!.Sorting.ShouldBe(sorting);
        capturedRequests[0]!.Filtering.ShouldBe(filtering);
        capturedRequests[1]!.Sorting.ShouldBe(sorting);
        capturedRequests[1]!.Filtering.ShouldBe(filtering);
    }

    [Test]
    public async Task AutoPage_FailedPage_ThrowsPingenApiErrorException()
    {
        var callCount = 0;
        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                return Task.FromResult(
                    callCount == 1
                        ? new ApiResult<CollectionResult<LetterData>>
                        {
                            IsSuccess = true,
                            Data = new CollectionResult<LetterData>(
                                [CreateLetterData("letter-1")],
                                new CollectionResultLinks("", "", "", "", ""),
                                new CollectionResultMeta(1, 3, 1, 1, 1, 3)
                            ),
                        }
                        : new ApiResult<CollectionResult<LetterData>> { IsSuccess = false }
                );
            });

        await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (var _ in _letterService.GetPageResultsAsync())
            {
                // consume pages
            }
        });
    }

    [Test]
    public async Task AutoPage_NullData_YieldsEmptyCollectionAndStops()
    {
        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterData>>(Arg.Any<string>(), Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ApiResult<CollectionResult<LetterData>> { IsSuccess = true, Data = null }));

        var pages = new List<IEnumerable<LetterData>>();
        await foreach (var page in _letterService.GetPageResultsAsync())
        {
            pages.Add(page);
        }

        pages.Count.ShouldBe(1);
        pages[0].ShouldBeEmpty();
    }

    private static LetterData CreateLetterData(string id) =>
        new()
        {
            Id = id,
            Type = "letters",
            Attributes = CreateLetterAttributes(),
            Relationships = CreateLetterRelationships(),
        };

    private static LetterDataDetailed CreateLetterDataDetailed(string id) =>
        new()
        {
            Id = id,
            Type = "letters",
            Attributes = CreateLetterAttributes(),
            Relationships = CreateLetterRelationships(),
            Meta = new(new(CreateLetterAbilities())),
        };

    private static Letter CreateLetterAttributes() =>
        new(
            Status: "draft",
            FileOriginalName: "test.pdf",
            FilePages: 1,
            Address: "Test Address",
            AddressPosition: LetterAddressPosition.left,
            Country: "CH",
            DeliveryProduct: "cheap",
            PrintMode: LetterPrintMode.simplex,
            PrintSpectrum: LetterPrintSpectrum.grayscale,
            PriceCurrency: "",
            PriceValue: null,
            PaperTypes: [],
            Fonts: [],
            TrackingNumber: "",
            SubmittedAt: null,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

    private static LetterRelationships CreateLetterRelationships() =>
        new(Organisation: new(new(""), null!), Events: new(new(new("", new(0)))), Batch: new(new(""), null!));

    private static LetterAbilities CreateLetterAbilities() =>
        new(
            Cancel: PingenApiAbility.ok,
            Delete: PingenApiAbility.ok,
            Submit: PingenApiAbility.ok,
            SendSimplex: PingenApiAbility.ok,
            Edit: PingenApiAbility.ok,
            GetPdfRaw: PingenApiAbility.ok,
            GetPdfValidation: PingenApiAbility.ok,
            ChangePaperType: PingenApiAbility.ok,
            ChangeWindowPosition: PingenApiAbility.ok,
            CreateCoverPage: PingenApiAbility.ok,
            FixOverwriteRestrictedAreas: PingenApiAbility.ok,
            FixCoverPage: PingenApiAbility.ok,
            FixRegularPaper: PingenApiAbility.ok
        );
}

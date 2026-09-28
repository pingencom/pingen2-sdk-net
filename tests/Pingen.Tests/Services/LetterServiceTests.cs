using Bogus;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.LetterPrices;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Letters.Views;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class LetterServiceTests : EndpointTestBase
{
    private static Faker<DataPost<LetterCreate, LetterCreateRelationships>> LetterCreateFaker()
    {
        return new Faker<DataPost<LetterCreate, LetterCreateRelationships>>()
            .RuleFor(x => x.Type, "letters")
            .RuleFor(
                x => x.Attributes,
                f => new LetterCreate
                {
                    FileOriginalName = f.System.FileName("pdf"),
                    FileUrl = f.Internet.Url(),
                    FileUrlSignature = f.Random.Hash(60),
                    AddressPosition = f.PickRandom<LetterAddressPosition>(),
                    AutoSend = f.Random.Bool(),
                    DeliveryProduct = f.PickRandom(
                        LetterCreateDeliveryProduct.Cheap,
                        LetterCreateDeliveryProduct.Fast,
                        LetterCreateDeliveryProduct.Bulk,
                        LetterCreateDeliveryProduct.Premium
                    ),
                    PrintMode = f.PickRandom<LetterPrintMode>(),
                    PrintSpectrum = f.PickRandom<LetterPrintSpectrum>(),
                }
            )
            .RuleFor(x => x.Relationships, f => LetterCreateRelationships.Create(f.Random.Guid().ToString()));
    }

    private static Faker<DataPatch<LetterSend>> LetterSendFaker(string letterId)
    {
        return new Faker<DataPatch<LetterSend>>()
            .RuleFor(x => x.Id, letterId)
            .RuleFor(x => x.Type, "letters")
            .RuleFor(
                x => x.Attributes,
                f => new LetterSend
                {
                    DeliveryProduct = f.PickRandom("cheap", "fast", "bulk", "premium"),
                    PrintMode = f.PickRandom<LetterPrintMode>(),
                    PrintSpectrum = f.PickRandom<LetterPrintSpectrum>(),
                }
            );
    }

    private static Faker<DataPost<LetterPriceConfiguration>> LetterPriceConfigurationFaker()
    {
        return new Faker<DataPost<LetterPriceConfiguration>>()
            .RuleFor(x => x.Type, "letter_price_calculator")
            .RuleFor(
                x => x.Attributes,
                f => new LetterPriceConfiguration
                {
                    Country = f.PickRandom("CH", "DE", "AT"),
                    PaperTypes = ["normal"],
                    PrintMode = f.PickRandom<LetterPrintMode>(),
                    PrintSpectrum = f.PickRandom<LetterPrintSpectrum>(),
                    DeliveryProduct = f.PickRandom("cheap", "fast", "bulk", "premium"),
                }
            );
    }

    [Test]
    public async Task GetPage_ShouldReturnLetters()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection());

        ApiResult<CollectionResult<LetterData>> result = await Client.Letters.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.FileOriginalName.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task GetPage_ShouldReturnEmptyWhenNoLetters()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0));

        ApiResult<CollectionResult<LetterData>> result = await Client.Letters.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(5, currentPage: 2, lastPage: 3));

        ApiResult<CollectionResult<LetterData>> result = await Client.Letters.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(2),
            () => result.Data!.Meta.LastPage.ShouldBe(3),
            () => result.Data!.Data.Count.ShouldBe(5)
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task GetPage_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError(OrgPath("deliveries/letters"), "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<CollectionResult<LetterData>> result = await Client.Letters.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors.Count.ShouldBeGreaterThan(0),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldAutoPaginate()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario("letters-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(1, currentPage: 1, lastPage: 2))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingGet())
            .InScenario("letters-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterCollection(1, currentPage: 2, lastPage: 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetPageResultsAsync_ShouldYieldSinglePage_WhenOnlyOneExists()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(2));

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterData> page in Client.Letters.GetPageResultsAsync())
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        Server.StubError(OrgPath("deliveries/letters"), "GET", PingenResponseFactory.ErrorResponse("Server Error", "Service unavailable", "500"), 500);

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
    public async Task Get_ShouldReturnSingleLetter()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}"), PingenResponseFactory.SingleLetter(letterId));

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Get(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(letterId),
            () => result.Data!.Data.Attributes.FileOriginalName.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Get_ShouldReturnErrorWhenNotFound()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}"),
            "GET",
            PingenResponseFactory.ErrorResponse("Not Found", "The letter does not exist.", "404"),
            404
        );

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Get(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Create_ShouldPostLetterAndReturnResult()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("deliveries/letters"), PingenResponseFactory.SingleLetter(letterId));

        DataPost<LetterCreate, LetterCreateRelationships>? data = LetterCreateFaker().Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(letterId),
            () => result.Data!.Data.Attributes.FileOriginalName.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task Create_ShouldReturnErrorWhenValidationFails()
    {
        Server.StubError(OrgPath("deliveries/letters"), "POST", PingenResponseFactory.ErrorResponse("Validation Failed", "FileUrl is required."));

        DataPost<LetterCreate, LetterCreateRelationships>? data = LetterCreateFaker().Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Create(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Validation Failed"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Send_ShouldPatchLetterAndReturnResult()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonPatch(OrgPath($"deliveries/letters/{letterId}/send"), PingenResponseFactory.SingleLetter(letterId));

        DataPatch<LetterSend>? data = LetterSendFaker(letterId).Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Send(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(letterId)
        );
    }

    [Test]
    public async Task Send_ShouldReturnErrorWhenAlreadySent()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/send"),
            "PATCH",
            PingenResponseFactory.ErrorResponse("Conflict", "Letter has already been sent.")
        );

        DataPatch<LetterSend>? data = LetterSendFaker(letterId).Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Send(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Conflict"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Send_ShouldReturnErrorWhenNotFound()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/send"),
            "PATCH",
            PingenResponseFactory.ErrorResponse("Not Found", "The letter does not exist.", "404"),
            404
        );

        DataPatch<LetterSend>? data = LetterSendFaker(letterId).Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Send(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task Cancel_ShouldPatchLetterCancel()
    {
        string letterId = Guid.NewGuid().ToString();

        Server
            .Given(Request.Create().WithPath(OrgPath($"deliveries/letters/{letterId}/cancel")).UsingPatch())
            .RespondWith(Response.Create().WithStatusCode(204).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        ApiResult result = await Client.Letters.Cancel(letterId);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Cancel_ShouldReturnErrorWhenAlreadyCancelled()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/cancel"),
            "PATCH",
            PingenResponseFactory.ErrorResponse("Conflict", "Letter has already been cancelled.")
        );

        ApiResult result = await Client.Letters.Cancel(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Conflict")
        );
    }

    [Test]
    public async Task Cancel_ShouldReturnErrorWhenNotFound()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/cancel"),
            "PATCH",
            PingenResponseFactory.ErrorResponse("Not Found", "The letter does not exist.", "404"),
            404
        );

        ApiResult result = await Client.Letters.Cancel(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found")
        );
    }

    [Test]
    public async Task Delete_ShouldReturnSuccess()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubDelete(OrgPath($"deliveries/letters/{letterId}"));

        ApiResult result = await Client.Letters.Delete(letterId);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Delete_ShouldReturnErrorWhenNotFound()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}"),
            "DELETE",
            PingenResponseFactory.ErrorResponse("Not Found", "The letter does not exist.", "404"),
            404
        );

        ApiResult result = await Client.Letters.Delete(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found")
        );
    }

    [Test]
    public async Task GetFileLocation_ShouldReturn302WithLocation()
    {
        string letterId = Guid.NewGuid().ToString();
        const string fileUrl = "https://s3.example.com/files/letter.pdf";

        Server
            .Given(Request.Create().WithPath(OrgPath($"deliveries/letters/{letterId}/file")).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(302).WithHeader("Location", fileUrl).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        ApiResult result = await Client.Letters.GetFileLocation(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Location.ShouldNotBeNull(),
            () => result.Location!.ToString().ShouldBe(fileUrl)
        );
    }

    [Test]
    public async Task GetFileLocation_ShouldReturnErrorWhenNotReady()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/file"),
            "GET",
            PingenResponseFactory.ErrorResponse("Not Found", "Letter file is not ready yet.", "404"),
            404
        );

        ApiResult result = await Client.Letters.GetFileLocation(letterId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Not Found")
        );
    }

    [Test]
    public async Task DownloadFileContent_ShouldReturnFileStream()
    {
        const string downloadPath = "/files/download/test-letter.pdf";
        byte[] fileContent = "fake-pdf-binary-content"u8.ToArray();

        Server
            .Given(Request.Create().WithPath(downloadPath).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/pdf").WithBody(fileContent));

        var fileUrl = new Uri($"{Server.Url}{downloadPath}");
        await using Stream stream = await Client.Letters.DownloadFileContent(fileUrl);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        byte[] downloadedContent = memoryStream.ToArray();

        downloadedContent.ShouldBe(fileContent);
    }

    [Test]
    public async Task DownloadFileContent_ShouldThrowWhenDownloadFails()
    {
        const string downloadPath = "/files/download/missing-letter.pdf";
        const string s3ErrorXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Error>
                <Code>NoSuchKey</Code>
                <Message>The specified key does not exist.</Message>
            </Error>
            """;

        Server
            .Given(Request.Create().WithPath(downloadPath).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404).WithHeader("Content-Type", "application/xml").WithBody(s3ErrorXml));

        var fileUrl = new Uri($"{Server.Url}{downloadPath}");

        PingenFileDownloadException exception = await Should.ThrowAsync<PingenFileDownloadException>(async () =>
            await Client.Letters.DownloadFileContent(fileUrl)
        );

        exception.ShouldNotBeNull();
    }

    [Test]
    public async Task CalculatePrice_ShouldReturnLetterPrice()
    {
        string priceId = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("deliveries/letters/price-calculator"), PingenResponseFactory.SingleLetterPriceCalculator(priceId, 2.75m), 200);

        DataPost<LetterPriceConfiguration>? data = LetterPriceConfigurationFaker().Generate();

        ApiResult<SingleResult<LetterPriceData>> result = await Client.Letters.CalculatePrice(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(priceId),
            () => result.Data!.Data.Attributes.Price.ShouldBe(2.75m),
            () => result.Data!.Data.Attributes.Currency.ShouldBe("CHF")
        );
    }

    [Test]
    public async Task CalculatePrice_ShouldReturnErrorWhenValidationFails()
    {
        Server.StubError(
            OrgPath("deliveries/letters/price-calculator"),
            "POST",
            PingenResponseFactory.ErrorResponse("Validation Failed", "Country is required.")
        );

        DataPost<LetterPriceConfiguration>? data = LetterPriceConfigurationFaker().Generate();

        ApiResult<SingleResult<LetterPriceData>> result = await Client.Letters.CalculatePrice(data);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.ApiError.ShouldNotBeNull(),
            () => result.ApiError!.Errors[0].Title.ShouldBe("Validation Failed"),
            () => result.Data.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetEventsPage_ShouldReturnLetterEvents()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}/events"), PingenResponseFactory.LetterEventCollection(letterId: letterId));

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetEventsPage(letterId, "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Code.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task GetEventsPage_ShouldReturnEmptyWhenNoEvents()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}/events"), PingenResponseFactory.LetterEventCollection(0, letterId));

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetEventsPage(letterId, "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetEventsPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}/events"), PingenResponseFactory.LetterEventCollection(4, letterId, 1, 3));

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetEventsPage(letterId, "en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(1),
            () => result.Data!.Meta.LastPage.ShouldBe(3)
        );
    }

    [Test]
    public async Task GetEventsPage_OnApiError_ShouldReturnUnsuccessfulApiResult()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/events"),
            "GET",
            PingenResponseFactory.ErrorResponse("Server Error", "Service unavailable", "500"),
            500
        );

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetEventsPage(letterId, "en-GB");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull(), () => result.Data.ShouldBeNull());
    }

    [Test]
    public async Task GetEventsPageResultsAsync_ShouldAutoPaginate()
    {
        string letterId = Guid.NewGuid().ToString();

        Server
            .Given(Request.Create().WithPath(OrgPath($"deliveries/letters/{letterId}/events")).UsingGet())
            .InScenario("letter-events-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterEventCollection(1, letterId, 1, 2))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath($"deliveries/letters/{letterId}/events")).UsingGet())
            .InScenario("letter-events-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterEventCollection(1, letterId, 2, 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetEventsPageResultsAsync(letterId, "en-GB"))
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetEventsPageResultsAsync_ShouldYieldSinglePage_WhenOnlyOneExists()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}/events"), PingenResponseFactory.LetterEventCollection(2, letterId));

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetEventsPageResultsAsync(letterId, "en-GB"))
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetEventsPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubError(
            OrgPath($"deliveries/letters/{letterId}/events"),
            "GET",
            PingenResponseFactory.ErrorResponse("Forbidden", "Access denied", "403"),
            403
        );

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<LetterEventData> _ in Client.LetterEvents.GetEventsPageResultsAsync(letterId, "en-GB"))
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task GetIssuesPage_ShouldReturnLetterIssues()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/issues"), PingenResponseFactory.LetterEventCollection());

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetIssuesPage("en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(3),
            () => result.Data!.Data[0].Attributes.Code.ShouldNotBeNullOrEmpty()
        );
    }

    [Test]
    public async Task GetIssuesPage_ShouldReturnEmptyWhenNoIssues()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/issues"), PingenResponseFactory.LetterEventCollection(0));

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetIssuesPage("en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetIssuesPage_ShouldExposePaginationMeta_ForMultiPageResponse()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/issues"), PingenResponseFactory.LetterEventCollection(4, currentPage: 1, lastPage: 2));

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetIssuesPage("en-GB");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data!.Meta.CurrentPage.ShouldBe(1),
            () => result.Data!.Meta.LastPage.ShouldBe(2)
        );
    }

    [Test]
    public async Task GetIssuesPage_OnApiError_ShouldReturnUnsuccessfulApiResult()
    {
        Server.StubError(
            OrgPath("deliveries/letters/events/issues"),
            "GET",
            PingenResponseFactory.ErrorResponse("Server Error", "Service unavailable", "500"),
            500
        );

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetIssuesPage("en-GB");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull(), () => result.Data.ShouldBeNull());
    }

    [Test]
    public async Task GetIssuesPageResultsAsync_ShouldAutoPaginate()
    {
        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters/events/issues")).UsingGet())
            .InScenario("letter-issues-paging")
            .WillSetStateTo("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterEventCollection(1, currentPage: 1, lastPage: 2))
            );

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters/events/issues")).UsingGet())
            .InScenario("letter-issues-paging")
            .WhenStateIs("page2")
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(PingenResponseFactory.LetterEventCollection(1, currentPage: 2, lastPage: 2))
            );

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetIssuesPageResultsAsync("en-GB"))
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetIssuesPageResultsAsync_ShouldYieldSinglePage_WhenOnlyOneExists()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/issues"), PingenResponseFactory.LetterEventCollection(2));

        var allItems = new List<string>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetIssuesPageResultsAsync("en-GB"))
            allItems.AddRange(page.Select(item => item.Id));

        allItems.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetIssuesPageResultsAsync_OnApiError_ShouldThrowPingenApiErrorException()
    {
        Server.StubError(OrgPath("deliveries/letters/events/issues"), "GET", PingenResponseFactory.ErrorResponse("Forbidden", "Access denied", "403"), 403);

        PingenApiErrorException exception = await Should.ThrowAsync<PingenApiErrorException>(async () =>
        {
            await foreach (IEnumerable<LetterEventData> _ in Client.LetterEvents.GetIssuesPageResultsAsync("en-GB"))
            {
                // Should never iterate
            }
        });

        exception.ApiResult.ShouldNotBeNull();
        exception.ApiResult!.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task GetSentPageResultsAsync_ShouldIterateEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/sent"), PingenResponseFactory.LetterEventCollection(2));

        var events = new List<LetterEventData>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetSentPageResultsAsync("en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetUndeliverablePageResultsAsync_ShouldIterateEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/undeliverable"), PingenResponseFactory.LetterEventCollection(2));

        var events = new List<LetterEventData>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetUndeliverablePageResultsAsync("en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(2);
    }

    [Test]
    public async Task GetDeliveredPageResultsAsync_ShouldIterateEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/delivered"), PingenResponseFactory.LetterEventCollection(2));

        var events = new List<LetterEventData>();
        await foreach (IEnumerable<LetterEventData> page in Client.LetterEvents.GetDeliveredPageResultsAsync("en-GB"))
            events.AddRange(page);

        events.Count.ShouldBe(2);
    }
}

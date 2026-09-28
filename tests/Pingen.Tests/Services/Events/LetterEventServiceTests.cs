using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Services.Connectors;

namespace Pingen.Tests.Services.Events;

public class LetterEventServiceTests
{
    private IPingenConnectionHandler _mockConnectionHandler = null!;
    private LetterEventService _letterEventService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConnectionHandler = Substitute.For<IPingenConnectionHandler>();
        _letterEventService = new LetterEventService(_mockConnectionHandler);
    }

    [Test]
    public async Task GetEventsPage_UrlEncodesLanguageParameter()
    {
        const string letterId = "test-letter-id";
        const string language = "en&foo=bar";
        var expectedPath = $"deliveries/letters/{letterId}/events?language={Uri.EscapeDataString(language)}";

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterEventData>>(expectedPath, Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(new ApiResult<CollectionResult<LetterEventData>> { IsSuccess = true });

        await _letterEventService.GetEventsPage(letterId, language);

        await _mockConnectionHandler
            .Received(1)
            .GetAsync<CollectionResult<LetterEventData>>(expectedPath, Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetIssuesPage_UrlEncodesLanguageParameter()
    {
        const string language = "en&foo=bar";
        var expectedPath = $"deliveries/letters/events/issues?language={Uri.EscapeDataString(language)}";

        _mockConnectionHandler
            .GetAsync<CollectionResult<LetterEventData>>(expectedPath, Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>())
            .Returns(new ApiResult<CollectionResult<LetterEventData>> { IsSuccess = true });

        await _letterEventService.GetIssuesPage(language);

        await _mockConnectionHandler
            .Received(1)
            .GetAsync<CollectionResult<LetterEventData>>(expectedPath, Arg.Any<ApiPagingRequest?>(), Arg.Any<CancellationToken>());
    }
}

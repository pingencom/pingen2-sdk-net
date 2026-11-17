using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.Services.Events;

[TestFixture]
public sealed class LetterEventCategoriesServiceTests : EndpointTestBase
{
    [Test]
    public async Task GetSentPage_ShouldReturnEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/sent"), PingenResponseFactory.LetterEventCollection());

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetSentPage("en-GB");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }

    [Test]
    public async Task GetUndeliverablePage_ShouldReturnEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/undeliverable"), PingenResponseFactory.LetterEventCollection());

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetUndeliverablePage("en-GB");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }

    [Test]
    public async Task GetDeliveredPage_ShouldReturnEvents()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters/events/delivered"), PingenResponseFactory.LetterEventCollection());

        ApiResult<CollectionResult<LetterEventData>> result = await Client.LetterEvents.GetDeliveredPage("en-GB");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Data!.Data.Count.ShouldBe(3));
    }
}

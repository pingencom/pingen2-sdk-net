using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services.Events;

[TestFixture]
public sealed class EventImageServiceTests : EndpointTestBase
{
    [Test]
    public async Task Letter_GetEventImage_ShouldReturn302WithLocation()
    {
        const string imageUrl = "https://s3.example.com/files/letter-event.png";
        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters/letter-1/events/event-1/image")).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(302).WithHeader("Location", imageUrl).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        ApiResult result = await Client.LetterEvents.GetEventImage("letter-1", "event-1");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Location.ShouldNotBeNull(),
            () => result.Location!.ToString().ShouldBe(imageUrl)
        );
    }

    [Test]
    public async Task Email_GetEventImage_ShouldReturn302WithLocation()
    {
        const string imageUrl = "https://s3.example.com/files/email-event.png";
        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/emails/email-1/events/event-1/image")).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(302).WithHeader("Location", imageUrl).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        ApiResult result = await Client.EmailEvents.GetEventImage("email-1", "event-1");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Location!.ToString().ShouldBe(imageUrl));
    }

    [Test]
    public async Task Ebill_GetEventImage_ShouldReturn302WithLocation()
    {
        const string imageUrl = "https://s3.example.com/files/ebill-event.png";
        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/ebills/ebill-1/events/event-1/image")).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(302).WithHeader("Location", imageUrl).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        ApiResult result = await Client.EbillEvents.GetEventImage("ebill-1", "event-1");

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.Location!.ToString().ShouldBe(imageUrl));
    }

    [Test]
    public async Task Letter_DownloadEventImage_ShouldReturnStream()
    {
        const string downloadPath = "/files/download/letter-event.png";
        byte[] content = "fake-png-binary"u8.ToArray();
        Server
            .Given(Request.Create().WithPath(downloadPath).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "image/png").WithBody(content));

        await using Stream stream = await Client.LetterEvents.DownloadEventImage(new Uri($"{Server.Url}{downloadPath}"));
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);

        buffer.ToArray().ShouldBe(content);
    }

    [Test]
    public async Task Email_DownloadEventImage_ShouldReturnStream()
    {
        const string downloadPath = "/files/download/email-event.png";
        byte[] content = "email-png"u8.ToArray();
        Server.Given(Request.Create().WithPath(downloadPath).UsingGet()).RespondWith(Response.Create().WithStatusCode(200).WithBody(content));

        await using Stream stream = await Client.EmailEvents.DownloadEventImage(new Uri($"{Server.Url}{downloadPath}"));
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);

        buffer.ToArray().ShouldBe(content);
    }

    [Test]
    public async Task Ebill_DownloadEventImage_ShouldThrowWhenDownloadFails()
    {
        const string downloadPath = "/files/download/missing.png";
        Server
            .Given(Request.Create().WithPath(downloadPath).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(403).WithBody("<Error><Code>AccessDenied</Code></Error>"));

        await Should.ThrowAsync<PingenFileDownloadException>(async () => await Client.EbillEvents.DownloadEventImage(new Uri($"{Server.Url}{downloadPath}")));
    }

    [Test]
    public async Task DownloadEventImage_EmptyErrorBody_ThrowsWithHttpStatusFallback()
    {
        // Empty storage error body: TryGetStorageErrorCode returns null, so the message falls back to the HTTP status.
        const string downloadPath = "/files/download/empty-error.png";
        Server.Given(Request.Create().WithPath(downloadPath).UsingGet()).RespondWith(Response.Create().WithStatusCode(500).WithBody(""));

        var exception = await Should.ThrowAsync<PingenFileDownloadException>(async () =>
            await Client.LetterEvents.DownloadEventImage(new Uri($"{Server.Url}{downloadPath}"))
        );
        exception.Message.ShouldContain("500");
    }

    [Test]
    public async Task DownloadEventImage_NonXmlErrorBody_ThrowsWithHttpStatusFallback()
    {
        // A non-XML storage error body (e.g. a gateway text page) must not surface an XmlException; it falls back to status.
        const string downloadPath = "/files/download/nonxml-error.png";
        Server
            .Given(Request.Create().WithPath(downloadPath).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(502).WithBody("upstream connect error or disconnect/reset before headers"));

        var exception = await Should.ThrowAsync<PingenFileDownloadException>(async () =>
            await Client.LetterEvents.DownloadEventImage(new Uri($"{Server.Url}{downloadPath}"))
        );
        exception.Message.ShouldContain("502");
    }
}

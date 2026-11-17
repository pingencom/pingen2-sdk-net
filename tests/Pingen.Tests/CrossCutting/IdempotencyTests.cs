using Bogus;
using Pingen.Abstractions.Constants;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Letters.Views;
using Pingen.Tests.Helpers;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Types;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class IdempotencyTests : EndpointTestBase
{
    private const string IdempotencyKeyHeader = "Idempotency-Key";

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

    [Test]
    public async Task Create_WithIdempotencyKey_ShouldSendIdempotencyKeyHeader()
    {
        string letterId = Guid.NewGuid().ToString();
        string idempotencyKey = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("deliveries/letters"), PingenResponseFactory.SingleLetter(letterId));

        DataPost<LetterCreate, LetterCreateRelationships> data = LetterCreateFaker().Generate();

        await Client.Letters.Create(data, idempotencyKey);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters") && e.RequestMessage?.Method == "POST");
        IDictionary<string, WireMockList<string>>? headers = entry.RequestMessage!.Headers!;
        headers.ShouldContainKey(IdempotencyKeyHeader);
        headers[IdempotencyKeyHeader][0].ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task Send_WithIdempotencyKey_ShouldSendIdempotencyKeyHeader()
    {
        string letterId = Guid.NewGuid().ToString();
        string idempotencyKey = Guid.NewGuid().ToString();

        Server.StubJsonPatch(OrgPath($"deliveries/letters/{letterId}/send"), PingenResponseFactory.SingleLetter(letterId));

        DataPatch<LetterSend> data = LetterSendFaker(letterId).Generate();

        await Client.Letters.Send(data, idempotencyKey);

        ILogEntry entry = Server.LogEntries.Single(e =>
            e.RequestMessage?.Path == OrgPath($"deliveries/letters/{letterId}/send") && e.RequestMessage?.Method == "PATCH"
        );
        IDictionary<string, WireMockList<string>>? headers = entry.RequestMessage!.Headers!;
        headers.ShouldContainKey(IdempotencyKeyHeader);
        headers[IdempotencyKeyHeader][0].ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task Cancel_WithIdempotencyKey_ShouldSendIdempotencyKeyHeader()
    {
        string letterId = Guid.NewGuid().ToString();
        string idempotencyKey = Guid.NewGuid().ToString();

        Server
            .Given(Request.Create().WithPath(OrgPath($"deliveries/letters/{letterId}/cancel")).UsingPatch())
            .RespondWith(Response.Create().WithStatusCode(204).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));

        await Client.Letters.Cancel(letterId, idempotencyKey);

        ILogEntry entry = Server.LogEntries.Single(e =>
            e.RequestMessage?.Path == OrgPath($"deliveries/letters/{letterId}/cancel") && e.RequestMessage?.Method == "PATCH"
        );
        IDictionary<string, WireMockList<string>>? headers = entry.RequestMessage!.Headers!;
        headers.ShouldContainKey(IdempotencyKeyHeader);
        headers[IdempotencyKeyHeader][0].ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task Create_WhenServerReplaysIdempotentResponse_ShouldSetIdempotentReplayedTrue()
    {
        string letterId = Guid.NewGuid().ToString();
        string idempotencyKey = Guid.NewGuid().ToString();

        Server
            .Given(Request.Create().WithPath(OrgPath("deliveries/letters")).UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithHeader(ApiHeaderNames.IdempotentReplayed, "true")
                    .WithBody(PingenResponseFactory.SingleLetter(letterId))
            );

        DataPost<LetterCreate, LetterCreateRelationships> data = LetterCreateFaker().Generate();

        ApiResult<SingleResult<LetterDataDetailed>> result = await Client.Letters.Create(data, idempotencyKey);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.IdempotentReplayed.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(letterId)
        );
    }

    [Test]
    public async Task Create_WithoutIdempotencyKey_ShouldNotSendIdempotencyKeyHeader()
    {
        string letterId = Guid.NewGuid().ToString();

        Server.StubJsonPost(OrgPath("deliveries/letters"), PingenResponseFactory.SingleLetter(letterId));

        DataPost<LetterCreate, LetterCreateRelationships> data = LetterCreateFaker().Generate();

        await Client.Letters.Create(data);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters") && e.RequestMessage?.Method == "POST");
        entry.RequestMessage!.Headers!.ShouldNotContainKey(IdempotencyKeyHeader);
    }
}

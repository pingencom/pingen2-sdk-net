using Pingen.Abstractions.Models.Letters;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class CancellationTokenTests : EndpointTestBase
{
    [Test]
    public async Task GetPage_WithCancelledToken_ShouldThrowAndNotSendRequest()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection());

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await Client.Batches.GetPage(cancellationToken: cts.Token));

        Server.VerifyNotCalled(OrgPath("batches"));
    }

    [Test]
    public async Task GetPageResultsAsync_WithCancelledToken_ShouldThrowAndNotSendRequest()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection());

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await foreach (IEnumerable<LetterData> _ in Client.Letters.GetPageResultsAsync(cancellationToken: cts.Token))
            {
                // Should never iterate
            }
        });

        Server.VerifyNotCalled(OrgPath("deliveries/letters"));
    }

    [Test]
    public async Task Get_WithCancelledToken_ShouldThrowAndNotSendRequest()
    {
        string batchId = Guid.NewGuid().ToString();
        Server.StubJsonGet(OrgPath($"batches/{batchId}"), PingenResponseFactory.SingleBatch(batchId));

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await Client.Batches.Get(batchId, cancellationToken: cts.Token));

        Server.VerifyNotCalled(OrgPath($"batches/{batchId}"));
    }
}

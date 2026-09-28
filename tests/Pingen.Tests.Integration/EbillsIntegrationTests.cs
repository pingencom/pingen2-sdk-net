using System.Text.Json;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Ebills;
using Pingen.Abstractions.Models.Ebills.Embedded;
using Pingen.Abstractions.Models.Ebills.Views;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class EbillsIntegrationTests : IntegrationSupport
{
    private const string Language = "en-GB";

    private string _ebillId = null!;

    private string _cancellableEbillId = null!;

    private string _deletionEbillId = null!;

    [Test]
    [Order(1)]
    public async Task ListEbills()
    {
        var collection = await Client.Ebills.GetPage();
        AssertSuccess(collection);
    }

    [Test]
    [Order(2)]
    public async Task CreateEbill()
    {
        var created = await CreateEbill(FileName, autoSend: false);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
        _ebillId = created.Data.Data.Id;
    }

    [Test]
    [Order(3)]
    public async Task GetEbillById()
    {
        _ = RequireCreated(_ebillId);

        var details = await Client.Ebills.Get(_ebillId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_ebillId);
        details.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(4)]
    public async Task GetEbillEvents()
    {
        _ = RequireCreated(_ebillId);

        var events = await Client.EbillEvents.GetEventsPage(_ebillId, Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(5)]
    public async Task GetEbillFile()
    {
        _ = RequireCreated(_ebillId);

        // Give the e-bill time to reach a retrievable state.
        await WaitFor(10);

        var file = await Client.Ebills.GetFileLocation(_ebillId);
        AssertSuccess(file);
        file.Location.ShouldNotBeNull();
    }

    [Test]
    [Order(6)]
    public async Task SendEbill()
    {
        _ = RequireCreated(_ebillId);

        // An e-bill can only be sent once it left the validating state.
        await WaitFor(10);

        var sent = await Client.Ebills.Send(_ebillId);
        AssertSuccess(sent);
    }

    [Test]
    [Order(7)]
    public async Task CreateCancellableEbill()
    {
        var created = await CreateEbill(FileNameCancellable, autoSend: true);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        _cancellableEbillId = created.Data.Data.Id;
    }

    [Test]
    [Order(8)]
    public async Task CancelEbill()
    {
        _ = RequireCreated(_cancellableEbillId);

        // Give the e-bill time to reach a cancellable state.
        await WaitFor(10);

        var cancelled = await Client.Ebills.Cancel(_cancellableEbillId);
        AssertSuccess(cancelled);

        var details = await Client.Ebills.Get(_cancellableEbillId);
        AssertSuccess(details);
        details.Data!.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(9)]
    public async Task CreateEbillForDeletion()
    {
        var created = await CreateEbill(FileName, autoSend: false);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        _deletionEbillId = created.Data.Data.Id;
    }

    [Test]
    [Order(10)]
    public async Task DeleteEbill()
    {
        _ = RequireCreated(_deletionEbillId);

        // Give the e-bill time to reach a deletable state.
        await WaitFor(10);

        var deleted = await Client.Ebills.Delete(_deletionEbillId);
        AssertSuccess(deleted);

        var afterDelete = await Client.Ebills.Get(_deletionEbillId);
        afterDelete.IsSuccess.ShouldBeFalse();
    }

    // Creates an e-bill, skipping the test when the channel is not configured for the organisation under test.
    private async Task<ApiResult<SingleResult<EbillDataDetailed>>> CreateEbill(string fileName, bool autoSend)
    {
        var created = await Client.Ebills.UploadAndCreate(OpenFixture(fileName), Ebill(fileName, autoSend));

        if (!created.IsSuccess && JsonSerializer.Serialize(created.ApiError).Contains("conflict_missing_configuration"))
            Assert.Ignore("E-Bill channel not configured - skipping.");

        return created;
    }

    private static EbillUpload Ebill(string fileName, bool autoSend) =>
        new()
        {
            FileOriginalName = fileName,
            AutoSend = autoSend,
            MetaData = new EbillMetaData
            {
                InvoiceNumber = $"INV-{Guid.NewGuid():N}"[..16],
                InvoiceDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                InvoiceDueDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"),
                RecipientIdentifier = "41100000014283293",
            },
        };
}

using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Enums.Letters;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.LetterPrices;
using Pingen.Abstractions.Models.Letters.Views;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class LettersIntegrationTests : IntegrationSupport
{
    private const string Language = "en-GB";

    private string _letterId = null!;

    private string _cancellableLetterId = null!;

    private string _deletionLetterId = null!;

    [Test]
    [Order(1)]
    public async Task ListLetters()
    {
        var collection = await Client.Letters.GetPage();
        AssertSuccess(collection);
    }

    [Test]
    [Order(2)]
    public async Task ListLettersPaginated()
    {
        var collection = await Client.Letters.GetPage(new ApiPagingRequest { PageNumber = 1, PageLimit = 3 });
        AssertSuccess(collection);
        collection.Data!.Data.Count.ShouldBeLessThanOrEqualTo(3);
    }

    [Test]
    [Order(3)]
    public async Task CreateLetter()
    {
        var created = await Client.Letters.UploadAndCreate(
            OpenFixture(FileName),
            Letter(FileName, addressPosition: LetterAddressPosition.left, autoSend: true)
        );
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Status.ShouldBe("validating");
        _letterId = created.Data.Data.Id;

        var details = await Client.Letters.Get(_letterId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_letterId);

        // Give the api time to make the new letter show up in the collection.
        await WaitFor(30);

        var collection = await Client.Letters.GetPage(
            new ApiPagingRequest
            {
                Sorting = [new KeyValuePair<string, CollectionSortDirection>("created_at", CollectionSortDirection.DESC)],
                PageNumber = 1,
                PageLimit = 20,
            }
        );
        AssertSuccess(collection);
        collection.Data!.Data.Select(item => item.Id).ShouldContain(_letterId, "Newest first, so the new letter must be on the first page.");
    }

    [Test]
    [Order(4)]
    public async Task GetLetterById()
    {
        _ = RequireCreated(_letterId);

        var details = await Client.Letters.Get(_letterId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_letterId);
        details.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(5)]
    public async Task GetLetterEvents()
    {
        _ = RequireCreated(_letterId);

        var events = await Client.LetterEvents.GetEventsPage(_letterId, Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(6)]
    public async Task GetLetterFile()
    {
        _ = RequireCreated(_letterId);

        var file = await Client.Letters.GetFileLocation(_letterId);
        AssertSuccess(file);
        file.Location.ShouldNotBeNull();

        await using var content = await Client.Letters.DownloadFileContent(file.Location!);
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer);
        buffer.Length.ShouldBeGreaterThan(0);
    }

    [Test]
    [Order(7)]
    public async Task GetLetterSentEvents()
    {
        var events = await Client.LetterEvents.GetSentPage(Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(8)]
    public async Task GetLetterDeliveredEvents()
    {
        var events = await Client.LetterEvents.GetDeliveredPage(Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(9)]
    public async Task GetLetterIssueEvents()
    {
        var events = await Client.LetterEvents.GetIssuesPage(Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(10)]
    public async Task GetLetterUndeliverableEvents()
    {
        var events = await Client.LetterEvents.GetUndeliverablePage(Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(11)]
    public async Task CalculateLetterPrice()
    {
        var price = await Client.Letters.CalculatePrice(
            new DataPost<LetterPriceConfiguration>
            {
                Attributes = new LetterPriceConfiguration
                {
                    Country = "CH",
                    PaperTypes = ["normal", "normal"],
                    PrintMode = LetterPrintMode.simplex,
                    PrintSpectrum = LetterPrintSpectrum.grayscale,
                    DeliveryProduct = LetterCreateDeliveryProduct.Cheap,
                },
            }
        );
        AssertSuccess(price);
        price.Data!.Data.Id.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(12)]
    public async Task CreateCancellableLetter()
    {
        var created = await Client.Letters.UploadAndCreate(
            OpenFixture(FileNameCancellable),
            Letter(FileNameCancellable, addressPosition: LetterAddressPosition.left, autoSend: true)
        );
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Status.ShouldBe("validating");
        _cancellableLetterId = created.Data.Data.Id;
    }

    [Test]
    [Order(13)]
    public async Task CancelLetter()
    {
        _ = RequireCreated(_cancellableLetterId);

        // Give the letter time to reach a cancellable state.
        await WaitFor(10);

        var cancelled = await Client.Letters.Cancel(_cancellableLetterId);
        AssertSuccess(cancelled);

        var details = await Client.Letters.Get(_cancellableLetterId);
        AssertSuccess(details);
        details.Data!.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(14)]
    public async Task CreateLetterForDeletion()
    {
        var created = await Client.Letters.UploadAndCreate(
            OpenFixture(FileName),
            Letter(FileName, addressPosition: LetterAddressPosition.right, autoSend: false)
        );
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Status.ShouldBe("validating");
        _deletionLetterId = created.Data.Data.Id;
    }

    [Test]
    [Order(15)]
    public async Task DeleteLetter()
    {
        _ = RequireCreated(_deletionLetterId);

        await WaitFor(5);

        var deleted = await Client.Letters.Delete(_deletionLetterId);
        AssertSuccess(deleted);

        var afterDelete = await Client.Letters.Get(_deletionLetterId);
        afterDelete.IsSuccess.ShouldBeFalse();
    }

    private static LetterUpload Letter(string fileName, LetterAddressPosition addressPosition, bool autoSend) =>
        new()
        {
            FileOriginalName = fileName,
            AddressPosition = addressPosition,
            AutoSend = autoSend,
            DeliveryProduct = LetterCreateDeliveryProduct.Cheap,
            PrintMode = LetterPrintMode.simplex,
            PrintSpectrum = LetterPrintSpectrum.grayscale,
        };
}

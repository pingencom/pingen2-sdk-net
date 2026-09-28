using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Emails;
using Pingen.Abstractions.Models.Emails.Embedded;
using Pingen.Abstractions.Models.Emails.Views;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class EmailsIntegrationTests : IntegrationSupport
{
    private const string Language = "en-GB";

    private string _emailId = null!;

    private string _cancellableEmailId = null!;

    private string _deletionEmailId = null!;

    [Test]
    [Order(1)]
    public async Task ListEmails()
    {
        var collection = await Client.Emails.GetPage();
        AssertSuccess(collection);
    }

    [Test]
    [Order(2)]
    public async Task CreateEmail()
    {
        var created = await CreateEmail(FileName, autoSend: true);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
        _emailId = created.Data.Data.Id;
    }

    [Test]
    [Order(3)]
    public async Task GetEmailById()
    {
        _ = RequireCreated(_emailId);

        var details = await Client.Emails.Get(_emailId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_emailId);
        details.Data.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(4)]
    public async Task GetEmailEvents()
    {
        _ = RequireCreated(_emailId);

        var events = await Client.EmailEvents.GetEventsPage(_emailId, Language);
        AssertSuccess(events);
    }

    [Test]
    [Order(5)]
    public async Task GetEmailFile()
    {
        _ = RequireCreated(_emailId);

        // Give the email time to reach a retrievable state.
        await WaitFor(5);

        var file = await Client.Emails.GetFileLocation(_emailId);
        AssertSuccess(file);
        file.Location.ShouldNotBeNull();
    }

    [Test]
    [Order(6)]
    public async Task CreateCancellableEmail()
    {
        var created = await CreateEmail(FileNameCancellable, autoSend: true);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        _cancellableEmailId = created.Data.Data.Id;
    }

    [Test]
    [Order(7)]
    public async Task CancelEmail()
    {
        _ = RequireCreated(_cancellableEmailId);

        // Give the email time to reach a cancellable state.
        await WaitFor(10);

        var cancelled = await Client.Emails.Cancel(_cancellableEmailId);
        AssertSuccess(cancelled);

        var details = await Client.Emails.Get(_cancellableEmailId);
        AssertSuccess(details);
        details.Data!.Data.Attributes.Status.ShouldNotBeNullOrEmpty();
    }

    [Test]
    [Order(8)]
    public async Task CreateEmailForDeletion()
    {
        var created = await CreateEmail(FileName, autoSend: false);
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        _deletionEmailId = created.Data.Data.Id;
    }

    [Test]
    [Order(9)]
    public async Task DeleteEmail()
    {
        _ = RequireCreated(_deletionEmailId);

        // Give the email time to reach a deletable state.
        await WaitFor(10);

        var deleted = await Client.Emails.Delete(_deletionEmailId);
        AssertSuccess(deleted);

        var afterDelete = await Client.Emails.Get(_deletionEmailId);
        afterDelete.IsSuccess.ShouldBeFalse();
    }

    private async Task<ApiResult<SingleResult<EmailDataDetailed>>> CreateEmail(string fileName, bool autoSend) =>
        await Client.Emails.UploadAndCreate(OpenFixture(fileName), Email(fileName, autoSend));

    private static EmailUpload Email(string fileName, bool autoSend) =>
        new()
        {
            FileOriginalName = fileName,
            AutoSend = autoSend,
            MetaData = new EmailMetaData
            {
                SenderName = "Pingen Test",
                RecipientEmail = "grzegorz.morgas@pingen.com",
                RecipientName = "Test Recipient",
                ReplyEmail = "noreply@example.com",
                ReplyName = "Reply Test",
                Subject = "Integration Test Email",
                Content = "Dear Recipient\n\nThis is an integration test.\n\nBest regards",
            },
        };
}

using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class AttachmentAndExtrasServiceTests : EndpointTestBase
{
    private static DataPost<AddAttachment> Attachment(string type) =>
        new()
        {
            Type = type,
            Attributes = new AddAttachment { FileUrl = "https://s3.example.com/upload/att", FileUrlSignature = "att-signature" },
        };

    [Test]
    public async Task Letter_AddAttachment_Succeeds()
    {
        Server.StubJsonPatch(OrgPath("deliveries/letters/letter-1/attachment"), "{}");

        ApiResult result = await Client.Letters.AddAttachment("letter-1", Attachment("letters"));

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Letter_AddAttachmentToMultipleLetters_Succeeds()
    {
        Server.StubJsonPatch(OrgPath("deliveries/letters/attachment"), "{}");

        var data = new DataPost<AddAttachmentToMultipleLetters>
        {
            Type = "letters_attachment",
            Attributes = new AddAttachmentToMultipleLetters
            {
                LetterIds = ["letter-1", "letter-2"],
                FileUrl = "https://s3.example.com/upload/att",
                FileUrlSignature = "att-signature",
            },
        };

        ApiResult result = await Client.Letters.AddAttachmentToMultipleLetters(data);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Batch_AddAttachment_Succeeds()
    {
        Server.StubJsonPatch(OrgPath("batches/batch-1/attachment"), "{}");

        ApiResult result = await Client.Batches.AddAttachment("batch-1", Attachment("batches"));

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Email_AddAttachment_Succeeds()
    {
        Server.StubJsonPatch(OrgPath("deliveries/emails/email-1/attachment"), "{}");

        ApiResult result = await Client.Emails.AddAttachment("email-1", Attachment("emails"));

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Ebill_AddAttachment_Succeeds()
    {
        Server.StubJsonPatch(OrgPath("deliveries/ebills/ebill-1/attachment"), "{}");

        ApiResult result = await Client.Ebills.AddAttachment("ebill-1", Attachment("ebills"));

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task User_GetAssociation_ReturnsAssociation()
    {
        Server.StubJsonGet("/user/associations/assoc-1", PingenResponseFactory.SingleUserAssociation("assoc-1"));

        ApiResult<SingleResult<UserAssociationDataDetailed>> result = await Client.Users.GetAssociation("assoc-1");

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data!.Data.Id.ShouldBe("assoc-1"),
            () => result.Data!.Data.Meta.ShouldNotBeNull()
        );
    }
}

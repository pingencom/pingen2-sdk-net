using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Webhooks;

namespace Pingen.Tests;

public class IncludeHelpers
{
    [Test]
    public void LetterIncludes_ConstantsMatchRelationshipNames()
    {
        "LetterIncludes".ShouldSatisfyAllConditions(() => LetterIncludes.Organisation.ShouldBe("organisation"), () => LetterIncludes.Batch.ShouldBe("batch"));
    }

    [Test]
    public void BatchIncludes_ConstantsMatchRelationshipNames()
    {
        BatchIncludes.Organisation.ShouldBe("organisation");
    }

    [Test]
    public void UserAssociationIncludes_ConstantsMatchRelationshipNames()
    {
        UserAssociationIncludes.Organisation.ShouldBe("organisation");
    }

    [Test]
    public void LetterEventIncludes_ConstantsMatchRelationshipNames()
    {
        LetterEventIncludes.Letter.ShouldBe("letter");
    }

    [Test]
    public void WebhookIncludes_ConstantsMatchRelationshipNames()
    {
        WebhookIncludes.Organisation.ShouldBe("organisation");
    }

    [Test]
    public void IncludeConstants_CanBeUsedInApiRequest()
    {
        var request = new ApiRequest { Include = [LetterIncludes.Organisation, LetterIncludes.Batch] };

        request.ShouldSatisfyAllConditions(
            () => request.Include.ShouldBe(new[] { "organisation", "batch" }, ignoreOrder: true),
            () => string.Join(',', request.Include!).ShouldBe("organisation,batch")
        );
    }
}

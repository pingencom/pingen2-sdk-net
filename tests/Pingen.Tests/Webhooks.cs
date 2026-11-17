using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Tests;

public class Webhooks
{
    [Test]
    public async Task DeserializeWebhookEventData()
    {
        var webhookBody = await File.ReadAllTextAsync("Assets/webhook_sample.json");
        webhookBody.ShouldNotBeEmpty();

        var webhookEventData = PingenSerialisationHelper.Deserialize<SingleResult<WebhookEventData>>(webhookBody);
        webhookEventData?.Data.Attributes.ShouldNotBeNull();

        var includedOrganisationFound = PingenSerialisationHelper.TryGetIncludedData(webhookEventData!, out Data<Organisation>? organisationData);
        webhookEventData!.ShouldSatisfyAllConditions(() => includedOrganisationFound.ShouldBeTrue(), () => organisationData.ShouldNotBeNull());

        var letterFound = PingenSerialisationHelper.TryGetIncludedData<Letter>(webhookEventData!, out var letterData);
        webhookEventData.ShouldSatisfyAllConditions(() => letterFound.ShouldBeTrue(), () => letterData.ShouldNotBeNull());

        Data<LetterEvent>? letterEventData;
        var letterEventFound = PingenSerialisationHelper.TryGetIncludedData(webhookEventData!, out letterEventData);
        webhookEventData.ShouldSatisfyAllConditions(() => letterEventFound.ShouldBeTrue(), () => letterEventData.ShouldNotBeNull());
    }
}

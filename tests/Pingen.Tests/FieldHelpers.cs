using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.LetterPrices;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.UserAssociations;
using Pingen.Abstractions.Models.Users;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Tests;

public class FieldHelpers
{
    [Test]
    public void LetterFields_ConstantsMatchJsonPropertyNames()
    {
        "LetterFields".ShouldSatisfyAllConditions(
            () => LetterFields.Status.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.Status)),
            () => LetterFields.FileOriginalName.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.FileOriginalName)),
            () => LetterFields.FilePages.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.FilePages)),
            () => LetterFields.Address.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.Address)),
            () => LetterFields.AddressPosition.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.AddressPosition)),
            () => LetterFields.Country.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.Country)),
            () => LetterFields.DeliveryProduct.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.DeliveryProduct)),
            () => LetterFields.PrintMode.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.PrintMode)),
            () => LetterFields.PrintSpectrum.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.PrintSpectrum)),
            () => LetterFields.PriceCurrency.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.PriceCurrency)),
            () => LetterFields.PriceValue.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.PriceValue)),
            () => LetterFields.PaperTypes.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.PaperTypes)),
            () => LetterFields.Fonts.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.Fonts)),
            () => LetterFields.TrackingNumber.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.TrackingNumber)),
            () => LetterFields.SubmittedAt.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.SubmittedAt)),
            () => LetterFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.CreatedAt)),
            () => LetterFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.UpdatedAt)),
            () => LetterFields.Source.ShouldBe(PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(x => x.Source))
        );
    }

    [Test]
    public void BatchFields_ConstantsMatchJsonPropertyNames()
    {
        "BatchFields".ShouldSatisfyAllConditions(
            () => BatchFields.Name.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.Name)),
            () => BatchFields.Icon.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.Icon)),
            () => BatchFields.Status.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.Status)),
            () => BatchFields.FileOriginalName.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.FileOriginalName)),
            () => BatchFields.LetterCount.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.LetterCount)),
            () => BatchFields.AddressPosition.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.AddressPosition)),
            () => BatchFields.PrintMode.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.PrintMode)),
            () => BatchFields.PrintSpectrum.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.PrintSpectrum)),
            () => BatchFields.PriceCurrency.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.PriceCurrency)),
            () => BatchFields.PriceValue.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.PriceValue)),
            () => BatchFields.SubmittedAt.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.SubmittedAt)),
            () => BatchFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.CreatedAt)),
            () => BatchFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.UpdatedAt)),
            () => BatchFields.DeliverableCount.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.DeliverableCount)),
            () => BatchFields.ChannelType.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.ChannelType)),
            () => BatchFields.Source.ShouldBe(PingenAttributesPropertyHelper<Batch>.GetJsonPropertyName(x => x.Source))
        );
    }

    [Test]
    public void OrganisationFields_ConstantsMatchJsonPropertyNames()
    {
        "OrganisationFields".ShouldSatisfyAllConditions(
            () => OrganisationFields.Name.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Name)),
            () => OrganisationFields.Status.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Status)),
            () => OrganisationFields.Plan.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Plan)),
            () => OrganisationFields.BillingMode.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.BillingMode)),
            () => OrganisationFields.BillingCurrency.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.BillingCurrency)),
            () => OrganisationFields.BillingBalance.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.BillingBalance)),
            () => OrganisationFields.DefaultCountry.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.DefaultCountry)),
            () =>
                OrganisationFields.DefaultAddressPosition.ShouldBe(
                    PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.DefaultAddressPosition)
                ),
            () =>
                OrganisationFields.DataRetentionAddresses.ShouldBe(
                    PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.DataRetentionAddresses)
                ),
            () => OrganisationFields.DataRetentionPdf.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.DataRetentionPdf)),
            () => OrganisationFields.Color.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Color)),
            () => OrganisationFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.CreatedAt)),
            () => OrganisationFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.UpdatedAt)),
            () => OrganisationFields.Edition.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Edition)),
            () => OrganisationFields.Flags.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.Flags)),
            () => OrganisationFields.MissingCredits.ShouldBe(PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.MissingCredits)),
            () =>
                OrganisationFields.LimitsMonthlyLettersCount.ShouldBe(
                    PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.LimitsMonthlyLettersCount)
                ),
            () =>
                OrganisationFields.LimitsMonthlyEmailsCount.ShouldBe(
                    PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.LimitsMonthlyEmailsCount)
                ),
            () =>
                OrganisationFields.LimitsMonthlyEbillsCount.ShouldBe(
                    PingenAttributesPropertyHelper<Organisation>.GetJsonPropertyName(x => x.LimitsMonthlyEbillsCount)
                )
        );
    }

    [Test]
    public void UserFields_ConstantsMatchJsonPropertyNames()
    {
        "UserFields".ShouldSatisfyAllConditions(
            () => UserFields.Email.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.Email)),
            () => UserFields.FirstName.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.FirstName)),
            () => UserFields.LastName.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.LastName)),
            () => UserFields.Status.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.Status)),
            () => UserFields.Language.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.Language)),
            () => UserFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.CreatedAt)),
            () => UserFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.UpdatedAt)),
            () => UserFields.Edition.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.Edition)),
            () => UserFields.Flags.ShouldBe(PingenAttributesPropertyHelper<User>.GetJsonPropertyName(x => x.Flags))
        );
    }

    [Test]
    public void UserAssociationFields_ConstantsMatchJsonPropertyNames()
    {
        "UserAssociationFields".ShouldSatisfyAllConditions(
            () => UserAssociationFields.Role.ShouldBe(PingenAttributesPropertyHelper<UserAssociation>.GetJsonPropertyName(x => x.Role)),
            () => UserAssociationFields.Status.ShouldBe(PingenAttributesPropertyHelper<UserAssociation>.GetJsonPropertyName(x => x.Status)),
            () => UserAssociationFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<UserAssociation>.GetJsonPropertyName(x => x.CreatedAt)),
            () => UserAssociationFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<UserAssociation>.GetJsonPropertyName(x => x.UpdatedAt))
        );
    }

    [Test]
    public void WebhookFields_ConstantsMatchJsonPropertyNames()
    {
        "WebhookFields".ShouldSatisfyAllConditions(
            () => WebhookFields.EventCategory.ShouldBe(PingenAttributesPropertyHelper<Webhook>.GetJsonPropertyName(x => x.EventCategory)),
            () => WebhookFields.Url.ShouldBe(PingenAttributesPropertyHelper<Webhook>.GetJsonPropertyName(x => x.Url)),
            () => WebhookFields.SigningKey.ShouldBe(PingenAttributesPropertyHelper<Webhook>.GetJsonPropertyName(x => x.SigningKey))
        );
    }

    [Test]
    public void LetterEventFields_ConstantsMatchJsonPropertyNames()
    {
        "LetterEventFields".ShouldSatisfyAllConditions(
            () => LetterEventFields.Code.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.Code)),
            () => LetterEventFields.Name.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.Name)),
            () => LetterEventFields.Producer.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.Producer)),
            () => LetterEventFields.Location.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.Location)),
            () => LetterEventFields.HasImage.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.HasImage)),
            () => LetterEventFields.Data.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.Data)),
            () => LetterEventFields.EmittedAt.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.EmittedAt)),
            () => LetterEventFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.CreatedAt)),
            () => LetterEventFields.UpdatedAt.ShouldBe(PingenAttributesPropertyHelper<LetterEvent>.GetJsonPropertyName(x => x.UpdatedAt))
        );
    }

    [Test]
    public void WebhookEventFields_ConstantsMatchJsonPropertyNames()
    {
        "WebhookEventFields".ShouldSatisfyAllConditions(
            () => WebhookEventFields.Reason.ShouldBe(PingenAttributesPropertyHelper<WebhookEvent>.GetJsonPropertyName(x => x.Reason)),
            () => WebhookEventFields.Url.ShouldBe(PingenAttributesPropertyHelper<WebhookEvent>.GetJsonPropertyName(x => x.Url)),
            () => WebhookEventFields.CreatedAt.ShouldBe(PingenAttributesPropertyHelper<WebhookEvent>.GetJsonPropertyName(x => x.CreatedAt)),
            () => WebhookEventFields.CorrectedAddress.ShouldBe(PingenAttributesPropertyHelper<WebhookEvent>.GetJsonPropertyName(x => x.CorrectedAddress))
        );
    }

    [Test]
    public void LetterPriceFields_ConstantsMatchJsonPropertyNames()
    {
        "LetterPriceFields".ShouldSatisfyAllConditions(
            () => LetterPriceFields.Currency.ShouldBe(PingenAttributesPropertyHelper<LetterPrice>.GetJsonPropertyName(x => x.Currency)),
            () => LetterPriceFields.Price.ShouldBe(PingenAttributesPropertyHelper<LetterPrice>.GetJsonPropertyName(x => x.Price))
        );
    }

    [Test]
    public void FieldConstants_CanBeUsedInApiRequest()
    {
        var request = new ApiRequest
        {
            SparseFieldsets = [new("letters", [LetterFields.Status, LetterFields.CreatedAt]), new("organisations", [OrganisationFields.Name])],
        };

        var letterEntry = request.SparseFieldsets!.First(e => e.Key == "letters");
        request.ShouldSatisfyAllConditions(
            () => request.SparseFieldsets.ShouldNotBeNull(),
            () => request.SparseFieldsets!.Count().ShouldBe(2),
            () => letterEntry.Value.ShouldBe(new[] { "status", "created_at" }, ignoreOrder: true)
        );
    }

    [Test]
    public void Letter_SparseDeserialization_ReturnsNullForMissingFields()
    {
        const string sparseJson = """{"status": "valid"}""";

        var letter = PingenSerialisationHelper.Deserialize<Letter>(sparseJson);

        letter!.ShouldSatisfyAllConditions(
            () => letter.ShouldNotBeNull(),
            () => letter!.Status.ShouldBe("valid"),
            () => letter!.FileOriginalName.ShouldBeNull(),
            () => letter!.Address.ShouldBeNull(),
            () => letter!.AddressPosition.ShouldBeNull(),
            () => letter!.Country.ShouldBeNull(),
            () => letter!.DeliveryProduct.ShouldBeNull(),
            () => letter!.PrintMode.ShouldBeNull(),
            () => letter!.PrintSpectrum.ShouldBeNull(),
            () => letter!.PriceCurrency.ShouldBeNull(),
            () => letter!.PaperTypes.ShouldBeNull(),
            () => letter!.Fonts.ShouldBeNull(),
            () => letter!.TrackingNumber.ShouldBeNull()
        );
    }

    [Test]
    public void Batch_SparseDeserialization_ReturnsNullForMissingFields()
    {
        const string sparseJson = """{"name": "test-batch", "status": "created"}""";

        var batch = PingenSerialisationHelper.Deserialize<Batch>(sparseJson);

        batch!.ShouldSatisfyAllConditions(
            () => batch.ShouldNotBeNull(),
            () => batch!.Name.ShouldBe("test-batch"),
            () => batch!.Status.ShouldBe("created"),
            () => batch!.Icon.ShouldBeNull(),
            () => batch!.FileOriginalName.ShouldBeNull(),
            () => batch!.AddressPosition.ShouldBeNull()
        );
    }
}

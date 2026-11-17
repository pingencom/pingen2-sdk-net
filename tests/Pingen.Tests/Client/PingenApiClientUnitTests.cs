using Pingen.Interfaces.Connectors;

namespace Pingen.Tests.Client;

public class PingenApiClientUnitTests
{
    [Test]
    public void PingenApiClient_ExposesAllServices()
    {
        var mockConnectionHandler = Substitute.For<IPingenConnectionHandler>();
        var mockLetters = Substitute.For<ILetterService>();
        var mockBatches = Substitute.For<IBatchService>();
        var mockEmails = Substitute.For<IEmailService>();
        var mockEbills = Substitute.For<IEbillService>();
        var mockUsers = Substitute.For<IUserService>();
        var mockOrganisations = Substitute.For<IOrganisationService>();
        var mockWebhooks = Substitute.For<IWebhookService>();
        var mockFiles = Substitute.For<IFilesService>();
        var mockLetterEvents = Substitute.For<ILetterEventService>();
        var mockBatchEvents = Substitute.For<IBatchEventService>();
        var mockEmailEvents = Substitute.For<IEmailEventService>();
        var mockEbillEvents = Substitute.For<IEbillEventService>();

        var client = new PingenApiClient(
            mockConnectionHandler,
            mockLetters,
            mockBatches,
            mockEmails,
            mockEbills,
            mockUsers,
            mockOrganisations,
            mockWebhooks,
            mockFiles,
            mockLetterEvents,
            mockBatchEvents,
            mockEmailEvents,
            mockEbillEvents
        );

        client.ShouldSatisfyAllConditions(
            () => client.Letters.ShouldBeSameAs(mockLetters),
            () => client.Batches.ShouldBeSameAs(mockBatches),
            () => client.Emails.ShouldBeSameAs(mockEmails),
            () => client.Ebills.ShouldBeSameAs(mockEbills),
            () => client.Users.ShouldBeSameAs(mockUsers),
            () => client.Organisations.ShouldBeSameAs(mockOrganisations),
            () => client.Webhooks.ShouldBeSameAs(mockWebhooks),
            () => client.Files.ShouldBeSameAs(mockFiles),
            () => client.LetterEvents.ShouldBeSameAs(mockLetterEvents),
            () => client.BatchEvents.ShouldBeSameAs(mockBatchEvents),
            () => client.EmailEvents.ShouldBeSameAs(mockEmailEvents),
            () => client.EbillEvents.ShouldBeSameAs(mockEbillEvents)
        );
    }

    [Test]
    public void SetOrganisationId_DelegatesToConnectionHandler()
    {
        var mockConnectionHandler = Substitute.For<IPingenConnectionHandler>();
        var client = new PingenApiClient(
            mockConnectionHandler,
            Substitute.For<ILetterService>(),
            Substitute.For<IBatchService>(),
            Substitute.For<IEmailService>(),
            Substitute.For<IEbillService>(),
            Substitute.For<IUserService>(),
            Substitute.For<IOrganisationService>(),
            Substitute.For<IWebhookService>(),
            Substitute.For<IFilesService>(),
            Substitute.For<ILetterEventService>(),
            Substitute.For<IBatchEventService>(),
            Substitute.For<IEmailEventService>(),
            Substitute.For<IEbillEventService>()
        );

        const string newOrgId = "new-org-id";
        client.SetOrganisationId(newOrgId);

        mockConnectionHandler.Received(1).SetOrganisationId(newOrgId);
    }

    [Test]
    public void Services_UseInitOnlySetters()
    {
        var serviceProperties = new[]
        {
            nameof(PingenApiClient.Letters),
            nameof(PingenApiClient.Batches),
            nameof(PingenApiClient.Emails),
            nameof(PingenApiClient.Ebills),
            nameof(PingenApiClient.Users),
            nameof(PingenApiClient.Organisations),
            nameof(PingenApiClient.Webhooks),
            nameof(PingenApiClient.Files),
            nameof(PingenApiClient.LetterEvents),
            nameof(PingenApiClient.BatchEvents),
            nameof(PingenApiClient.EmailEvents),
            nameof(PingenApiClient.EbillEvents),
        };

        foreach (var propertyName in serviceProperties)
        {
            var property = typeof(PingenApiClient).GetProperty(propertyName)!;
            var setMethod = property.GetSetMethod()!;
            var modifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();
            modifiers.ShouldContain(typeof(System.Runtime.CompilerServices.IsExternalInit), $"{propertyName} should be init-only");
        }
    }
}

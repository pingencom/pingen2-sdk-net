using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;

namespace Pingen.Services;

/// <inheritdoc />
public sealed class PingenApiClient : IPingenApiClient
{
    /// <summary>
    /// Instance of connection handler used for all services
    /// </summary>
    private readonly IPingenConnectionHandler _pingenConnectionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingenApiClient"/> class.
    /// </summary>
    public PingenApiClient(
        IPingenConnectionHandler pingenConnectionHandler,
        ILetterService letterService,
        IBatchService batchService,
        IEmailService emailService,
        IEbillService ebillService,
        IUserService userService,
        IOrganisationService organisationService,
        IWebhookService webhookService,
        IFilesService filesService,
        ILetterEventService letterEventService,
        IBatchEventService batchEventService,
        IEmailEventService emailEventService,
        IEbillEventService ebillEventService
    )
    {
        _pingenConnectionHandler = pingenConnectionHandler;

        Letters = letterService;
        Batches = batchService;
        Emails = emailService;
        Ebills = ebillService;
        Users = userService;
        Organisations = organisationService;
        Webhooks = webhookService;
        Files = filesService;
        LetterEvents = letterEventService;
        BatchEvents = batchEventService;
        EmailEvents = emailEventService;
        EbillEvents = ebillEventService;
    }

    /// <inheritdoc />
    public void SetOrganisationId(string organisationId)
    {
        _pingenConnectionHandler.SetOrganisationId(organisationId);
    }

    /// <inheritdoc />
    public ILetterService Letters { get; init; }

    /// <inheritdoc />
    public IBatchService Batches { get; init; }

    /// <inheritdoc />
    public IEmailService Emails { get; init; }

    /// <inheritdoc />
    public IEbillService Ebills { get; init; }

    /// <inheritdoc />
    public IUserService Users { get; init; }

    /// <inheritdoc />
    public IOrganisationService Organisations { get; init; }

    /// <inheritdoc />
    public IWebhookService Webhooks { get; init; }

    /// <inheritdoc />
    public IFilesService Files { get; init; }

    /// <inheritdoc />
    public ILetterEventService LetterEvents { get; init; }

    /// <inheritdoc />
    public IBatchEventService BatchEvents { get; init; }

    /// <inheritdoc />
    public IEmailEventService EmailEvents { get; init; }

    /// <inheritdoc />
    public IEbillEventService EbillEvents { get; init; }
}

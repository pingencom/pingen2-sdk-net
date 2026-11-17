using Pingen.Interfaces.Connectors;

namespace Pingen.Interfaces;

/// <summary>
/// Connector service to call Pingen REST API. <see href="https://api.pingen.com/documentation">API Doc</see>
/// </summary>
public interface IPingenApiClient
{
    /// <summary>
    /// Change the organisation ID to use for upcoming requests
    /// </summary>
    /// <param name="organisationId">Id to use for all requests at /organisations/{organisationId}/*</param>
    public void SetOrganisationId(string organisationId);

    /// <summary>
    /// Pingen batches connector. <see href="https://api.pingen.com/documentation#tag/batches.general">API Doc - Batches</see>
    /// </summary>
    public IBatchService Batches { get; init; }

    /// <summary>
    /// Pingen letters connector. <see href="https://api.pingen.com/documentation#tag/deliveries.letters.general">API Doc - Letters</see>
    /// </summary>
    public ILetterService Letters { get; init; }

    /// <summary>
    /// Pingen emails connector. <see href="https://api.pingen.com/documentation#tag/deliveries.emails.general">API Doc - Emails</see>
    /// </summary>
    public IEmailService Emails { get; init; }

    /// <summary>
    /// Pingen ebills connector. <see href="https://api.pingen.com/documentation#tag/deliveries.ebills.general">API Doc - Ebills</see>
    /// </summary>
    public IEbillService Ebills { get; init; }

    /// <summary>
    /// Pingen users connector. <see href="https://api.pingen.com/documentation#tag/user.general">API Doc - Users</see>
    /// </summary>
    public IUserService Users { get; init; }

    /// <summary>
    /// Pingen organisations connector. <see href="https://api.pingen.com/documentation#tag/organisations.general">API Doc - Organisations</see>
    /// </summary>
    public IOrganisationService Organisations { get; init; }

    /// <summary>
    /// Pingen webhooks connector. <see href="https://api.pingen.com/documentation#tag/organisations.management.webhooks">API Doc - Webhooks</see>
    /// </summary>
    public IWebhookService Webhooks { get; init; }

    /// <summary>
    /// Pingen files connector. <see href="https://api.pingen.com/documentation#tag/misc.files">API Doc - Files</see>
    /// </summary>
    public IFilesService Files { get; init; }

    /// <summary>
    /// Pingen letter events connector. <see href="https://api.pingen.com/documentation#tag/letters.events">API Doc - Letter events</see>
    /// </summary>
    public ILetterEventService LetterEvents { get; init; }

    /// <summary>
    /// Pingen batch events connector. <see href="https://api.pingen.com/documentation#tag/batches.events">API Doc - Batch events</see>
    /// </summary>
    public IBatchEventService BatchEvents { get; init; }

    /// <summary>
    /// Pingen email events connector. <see href="https://api.pingen.com/documentation#tag/emails.events">API Doc - Email events</see>
    /// </summary>
    public IEmailEventService EmailEvents { get; init; }

    /// <summary>
    /// Pingen ebill events connector. <see href="https://api.pingen.com/documentation#tag/ebills.events">API Doc - Ebill events</see>
    /// </summary>
    public IEbillEventService EbillEvents { get; init; }
}

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Attachments;
using Pingen.Abstractions.Models.Emails;
using Pingen.Abstractions.Models.Emails.Views;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services.Connectors.Base;
using Pingen.Services.Connectors.Endpoints;

namespace Pingen.Services.Connectors;

/// <inheritdoc cref="Pingen.Interfaces.Connectors.IEmailService" />
public sealed class EmailService : ConnectorService, IEmailService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    public EmailService(IPingenConnectionHandler connectionHandler)
        : base(connectionHandler) { }

    /// <inheritdoc />
    public async Task<ApiResult<CollectionResult<EmailData>>> GetPage(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<CollectionResult<EmailData>>(EmailsEndpoints.Root, apiPagingRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEnumerable<EmailData>> GetPageResultsAsync(
        [Optional] ApiPagingRequest? apiPagingRequest,
        [EnumeratorCancellation] [Optional] CancellationToken cancellationToken
    )
    {
        await foreach (
            var page in AutoPage(apiPagingRequest, async apiRequest => await GetPage(apiRequest, cancellationToken)).WithCancellation(cancellationToken)
        )
            yield return page;
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<EmailDataDetailed>>> Create(
        DataPost<EmailCreate, EmailCreateRelationships> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "emails" };

        return await ConnectionHandler.PostAsync<SingleResult<EmailDataDetailed>, DataPost<EmailCreate, EmailCreateRelationships>>(
            EmailsEndpoints.Root,
            data,
            idempotencyKey,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<EmailDataDetailed>>> Get(
        string emailId,
        [Optional] ApiRequest? apiRequest,
        [Optional] CancellationToken cancellationToken
    )
    {
        return await ConnectionHandler.GetAsync<SingleResult<EmailDataDetailed>>(
            requestPath: EmailsEndpoints.Single(emailId),
            apiRequest: apiRequest,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResult> Cancel(string emailId, [Optional] string? idempotencyKey, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.PatchAsync(EmailsEndpoints.Cancel(emailId), idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> Delete(string emailId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.DeleteAsync(EmailsEndpoints.Single(emailId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult> GetFileLocation(string emailId, [Optional] CancellationToken cancellationToken)
    {
        return await ConnectionHandler.GetAsync(requestPath: EmailsEndpoints.File(emailId), apiPagingRequest: null, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadFileContent(Uri fileUrl, [Optional] CancellationToken cancellationToken) =>
        await DownloadFileContentInternal(fileUrl, cancellationToken);

    /// <inheritdoc />
    public async Task<ApiResult> AddAttachment(
        string emailId,
        DataPost<AddAttachment> data,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        data = data with { Type = "emails" };

        return await ConnectionHandler.PatchAsync(EmailsEndpoints.Attachment(emailId), data, idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResult<SingleResult<EmailDataDetailed>>> UploadAndCreate(
        Stream file,
        EmailUpload attributes,
        [Optional] EmailCreateRelationships? relationships,
        [Optional] string? idempotencyKey,
        [Optional] CancellationToken cancellationToken
    )
    {
        FileUploadData fileUploadData = await RequestAndUploadFile(file, cancellationToken);

        return await Create(
            new DataPost<EmailCreate, EmailCreateRelationships>
            {
                Type = "emails",
                Attributes = new EmailCreate
                {
                    FileOriginalName = attributes.FileOriginalName,
                    FileUrl = fileUploadData.Attributes.Url,
                    FileUrlSignature = fileUploadData.Attributes.UrlSignature,
                    AutoSend = attributes.AutoSend,
                    MetaData = attributes.MetaData,
                },
                Relationships = relationships,
            },
            idempotencyKey,
            cancellationToken
        );
    }
}

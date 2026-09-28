using System.Runtime.InteropServices;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Files;

namespace Pingen.Interfaces.Connectors;

/// <summary>
/// Pingen letter service endpoint. <see href="https://api.pingen.com/documentation#tag/misc.files">API Doc - Files</see>
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Get details for file upload. <see href="https://api.pingen.com/documentation#tag/misc.files/operation/files.file-upload">API Doc - Files details</see>
    /// </summary>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    public Task<ApiResult<SingleResult<FileUploadData>>> GetPath([Optional] CancellationToken cancellationToken);

    /// <summary>
    /// Upload a file to URL received in result from <see cref="GetPath"/>
    /// </summary>
    /// <param name="fileUploadData">Result from <see cref="GetPath"/></param>
    /// <param name="data">Binary file to upload as stream</param>
    /// <param name="cancellationToken">Optional, A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns>An <see cref="ExternalRequestResult"/> indicating the outcome of the upload</returns>
    public Task<ExternalRequestResult> UploadFile(FileUploadData fileUploadData, Stream data, [Optional] CancellationToken cancellationToken);
}

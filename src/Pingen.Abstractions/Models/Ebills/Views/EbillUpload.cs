using Pingen.Abstractions.Models.Ebills.Embedded;

namespace Pingen.Abstractions.Models.Ebills.Views;

/// <summary>
/// Input for the <c>UploadAndCreate</c> convenience flow - the ebill create attributes without
/// <c>file_url</c> / <c>file_url_signature</c>, which are filled in automatically after the file is uploaded.
/// </summary>
public sealed record EbillUpload
{
    /// <summary>Filename [ 5 .. 255 ] characters.</summary>
    public required string FileOriginalName { get; init; }

    /// <summary>Auto send.</summary>
    public required bool AutoSend { get; init; }

    /// <summary>Meta data (invoice number, dates, recipient identifier).</summary>
    public EbillMetaData? MetaData { get; init; }
}

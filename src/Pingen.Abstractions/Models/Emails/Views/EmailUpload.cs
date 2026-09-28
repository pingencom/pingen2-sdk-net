using Pingen.Abstractions.Models.Emails.Embedded;

namespace Pingen.Abstractions.Models.Emails.Views;

/// <summary>
/// Input for the <c>UploadAndCreate</c> convenience flow - the email create attributes without
/// <c>file_url</c> / <c>file_url_signature</c>, which are filled in automatically after the file is uploaded.
/// </summary>
public sealed record EmailUpload
{
    /// <summary>Filename [ 5 .. 255 ] characters.</summary>
    public required string FileOriginalName { get; init; }

    /// <summary>Auto send.</summary>
    public required bool AutoSend { get; init; }

    /// <summary>Meta data (sender, recipient, subject, body). Required to actually deliver the email.</summary>
    public EmailMetaData? MetaData { get; init; }
}

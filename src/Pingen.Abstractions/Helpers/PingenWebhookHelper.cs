using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.Webhooks.WebhookEvents;

namespace Pingen.Abstractions.Helpers;

/// <summary>
/// Helper methods for Pingen Webhooks
/// </summary>
public static class PingenWebhookHelper
{
    /// <summary>
    /// Validate webhook signature and get data from stream (request body)
    /// </summary>
    /// <returns>webhook event, included organisation, included letter, included letter event</returns>
    /// <exception cref="PingenWebhookValidationErrorException"></exception>
    public static async Task<(
        WebhookEventData? webhookEventData,
        Data<Organisation>? organisationData,
        Data<Letter>? letterData,
        Data<LetterEvent>? letterEventData
    )> ValidateWebhookAndGetData(string signingKey, string signature, Stream requestStream, [Optional] CancellationToken cancellationToken)
    {
        // leaveOpen: the request body belongs to the caller (e.g. ASP.NET Request.Body) and must stay usable.
        using var reader = new StreamReader(requestStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        requestStream.Position = 0;

        if (!await ValidateWebhook(signingKey, signature, requestStream, cancellationToken))
            throw new PingenWebhookValidationErrorException(
                PingenSerialisationHelper.Deserialize<WebhookEventData>(payload),
                "Validation of webhook signature failed"
            );

        var webhookEventData = PingenSerialisationHelper.Deserialize<SingleResult<WebhookEventData>>(payload);
        PingenSerialisationHelper.TryGetIncludedData(webhookEventData!, out Data<Organisation>? organisationData);
        PingenSerialisationHelper.TryGetIncludedData(webhookEventData!, out Data<Letter>? letterData);
        PingenSerialisationHelper.TryGetIncludedData(webhookEventData!, out Data<LetterEvent>? letterEventData);

        return (webhookEventData?.Data, organisationData, letterData, letterEventData);
    }

    /// <summary>
    /// Validate webhook signature
    /// </summary>
    public static async Task<bool> ValidateWebhook(string signingKey, string signature, Stream requestStream, [Optional] CancellationToken cancellationToken)
    {
        var keyByte = Encoding.UTF8.GetBytes(signingKey);
        using var hmacSha256 = new HMACSHA256(keyByte);
        var hash = await hmacSha256.ComputeHashAsync(requestStream, cancellationToken);

        try
        {
            var signatureBytes = Convert.FromHexString(signature);
            return CryptographicOperations.FixedTimeEquals(hash, signatureBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Helpers.JsonConverters;

/// <summary>
/// Serialises <see cref="DateTime"/> in the Pingen wire format (<c>Y-m-d\TH:i:sO</c>, e.g.
/// <c>2020-11-19T09:42:48+0100</c>). The API emits the UTC offset without a colon, so reading uses a tolerant
/// parser that accepts both the colon-less (<c>+0100</c>) and colon (<c>+01:00</c>) forms plus <c>Z</c>.
/// </summary>
internal sealed class PingenDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Wire format the API documents for datetimes (<c>Y-m-d\TH:i:sO</c>): a colon-less UTC offset. Values are
    /// normalised to UTC before writing, so the offset is always <c>+0000</c>.
    /// </summary>
    internal const string PingenDateTimeFormat = "yyyy-MM-ddTHH:mm:ss'+0000'";

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ReadDateTime(reader.GetString()) ?? DateTime.MinValue;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(Format(value));

    /// <summary>
    /// Parses a Pingen datetime string to a UTC <see cref="DateTime"/>. Accepts colon-less (<c>+0100</c>) and
    /// colon (<c>+01:00</c>) offsets as well as <c>Z</c>; a value without an offset is assumed to be UTC.
    /// Returns <see langword="null"/> for a null/empty or unparseable value.
    /// </summary>
    internal static DateTime? ReadDateTime(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed)
            ? parsed.UtcDateTime
            : null;
    }

    /// <summary>
    /// Formats a <see cref="DateTime"/> as a UTC Pingen datetime string (colon-less <c>+0000</c> offset).
    /// </summary>
    internal static string Format(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };

        return utc.ToString(PingenDateTimeFormat, CultureInfo.InvariantCulture);
    }
}

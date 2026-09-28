using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Helpers.JsonConverters;

/// <summary>
/// Nullable counterpart of <see cref="PingenDateTimeConverter"/>: reads a null/empty value as
/// <see langword="null"/> and otherwise shares the same tolerant parsing and colon-less UTC formatting.
/// </summary>
internal sealed class PingenDateTimeNullableConverter : JsonConverter<DateTime?>
{
    /// <inheritdoc />
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        PingenDateTimeConverter.ReadDateTime(reader.GetString());

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(PingenDateTimeConverter.Format(value.Value));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

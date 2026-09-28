using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pingen.Abstractions.Helpers.JsonConverters;

/// <summary>
/// Pingen key value pair converter
/// </summary>
internal sealed class PingenKeyValuePairStringObjectConverter : JsonConverter<KeyValuePair<string, object>>
{
    /// <inheritdoc />
    public override KeyValuePair<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var valueString = reader.GetString();

        if (string.IsNullOrEmpty(valueString))
            return default;

        return JsonSerializer.Deserialize<Dictionary<string, object>>(valueString, options)?.First() ?? default;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, KeyValuePair<string, object> value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(JsonSerializer.Serialize(new Dictionary<string, object>(new[] { value }), options));
    }
}

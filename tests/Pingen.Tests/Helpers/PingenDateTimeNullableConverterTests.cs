using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Helpers.JsonConverters;

namespace Pingen.Tests.Helpers;

public class PingenDateTimeNullableConverterTests
{
    [Test]
    public void Serialize_WithValue_UsesPingenFormat()
    {
        var obj = new NullableDateTimeHolder { Date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldContain("2024-06-15T14:30:00");
    }

    [Test]
    public void Deserialize_ValidDateString_ReturnsDateTime()
    {
        string json = "{\"date\":\"2024-06-15T14:30:00+00:00\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(
            () => result.Date!.Value.Year.ShouldBe(2024),
            () => result.Date!.Value.Month.ShouldBe(6),
            () => result.Date!.Value.Day.ShouldBe(15)
        );
    }

    [Test]
    public void Deserialize_NullToken_ReturnsNull()
    {
        string json = "{\"date\":null}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldBeNull();
    }

    [Test]
    public void Deserialize_EmptyString_ReturnsNull()
    {
        string json = "{\"date\":\"\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldBeNull();
    }

    [Test]
    public void Serialize_NullValue_OmitsProperty()
    {
        var obj = new NullableDateTimeHolder { Date = null };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldNotContain("date");
    }

    [Test]
    public void RoundTrip_SerializeThenDeserialize_PreservesDate()
    {
        var original = new NullableDateTimeHolder { Date = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(original);
        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(
            () => result.Date!.Value.Year.ShouldBe(2024),
            () => result.Date!.Value.Month.ShouldBe(6),
            () => result.Date!.Value.Day.ShouldBe(15)
        );
    }

    [Test]
    public void Deserialize_NullableMinValue_ReturnsMinValue()
    {
        string json = "{\"date\":\"0001-01-01T00:00:00+00:00\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldBe(DateTime.MinValue);
    }

    [Test]
    public void Deserialize_NullableMaxValueFormatted_RoundTripsApproximately()
    {
        var obj = new NullableDateTimeHolder { Date = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);
        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(
            () => result.Date!.Value.Year.ShouldBe(9999),
            () => result.Date!.Value.Month.ShouldBe(12),
            () => result.Date!.Value.Day.ShouldBe(31),
            () => result.Date!.Value.Hour.ShouldBe(23),
            () => result.Date!.Value.Minute.ShouldBe(59),
            () => result.Date!.Value.Second.ShouldBe(59)
        );
    }

    [Test]
    public void Deserialize_InvalidString_ReturnsNull()
    {
        string json = "{\"date\":\"garbage\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldBeNull();
    }

    [Test]
    public void Read_DirectInvocation_NullToken_ReturnsNull()
    {
        var converter = new PingenDateTimeNullableConverter();
        var opts = new JsonSerializerOptions();
        byte[] bytes = "null"u8.ToArray();
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        DateTime? result = converter.Read(ref reader, typeof(DateTime?), opts);

        result.ShouldBeNull();
    }

    [Test]
    public void Write_DirectInvocation_NullValue_WritesJsonNull()
    {
        var converter = new PingenDateTimeNullableConverter();
        var opts = new JsonSerializerOptions();
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms))
        {
            converter.Write(writer, null, opts);
        }

        Encoding.UTF8.GetString(ms.ToArray()).ShouldBe("null");
    }

    [Test]
    public void RoundTrip_PreservesNullableValue()
    {
        var obj = new NullableDateTimeHolder { Date = new DateTime(2024, 7, 1, 12, 0, 0, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);
        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(
            () => result.Date!.Value.Year.ShouldBe(2024),
            () => result.Date!.Value.Month.ShouldBe(7),
            () => result.Date!.Value.Day.ShouldBe(1)
        );
    }

    private sealed record NullableDateTimeHolder
    {
        [JsonPropertyName("date")]
        public DateTime? Date { get; init; }
    }
}

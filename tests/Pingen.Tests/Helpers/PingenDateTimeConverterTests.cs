using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Helpers.JsonConverters;

namespace Pingen.Tests.Helpers;

public class PingenDateTimeConverterTests
{
    [Test]
    public void Serialize_DateTime_UsesPingenFormat()
    {
        var obj = new DateTimeHolder { Date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldContain("2024-06-15T14:30:00");
    }

    [Test]
    public void Deserialize_PingenDateTimeFormat_ReturnsCorrectDateTime()
    {
        string json = "{\"date\":\"2024-06-15T14:30:00+00:00\"}";

        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(() => result.Date.Year.ShouldBe(2024), () => result.Date.Month.ShouldBe(6), () => result.Date.Day.ShouldBe(15));
    }

    [Test]
    public void Deserialize_EmptyDateTimeString_ReturnsMinValue()
    {
        string json = "{\"date\":\"\"}";

        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result.Date.ShouldBe(DateTime.MinValue);
    }

    [Test]
    public void Serialize_NullableDateTime_OmitsWhenNull()
    {
        var obj = new NullableDateTimeHolder { Date = null };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldNotContain("date");
    }

    [Test]
    public void Deserialize_DateTimeMinValue_RoundTripsThroughHelper()
    {
        var obj = new DateTimeHolder { Date = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);
        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result.Date.ShouldBe(DateTime.MinValue);
    }

    [Test]
    public void Deserialize_DateTimeMaxValue_RoundTripsApproximately()
    {
        var obj = new DateTimeHolder { Date = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);
        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(
            () => result.Date.Year.ShouldBe(9999),
            () => result.Date.Month.ShouldBe(12),
            () => result.Date.Day.ShouldBe(31),
            () => result.Date.Hour.ShouldBe(23),
            () => result.Date.Minute.ShouldBe(59),
            () => result.Date.Second.ShouldBe(59)
        );
    }

    [Test]
    public void Deserialize_InvalidString_ReturnsMinValue()
    {
        string json = "{\"date\":\"not-a-date\"}";

        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result.Date.ShouldBe(DateTime.MinValue);
    }

    [Test]
    public void Deserialize_TimezoneOffset_ConvertsToUtc()
    {
        string json = "{\"date\":\"2024-06-15T14:30:00+02:00\"}";

        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(
            () => result.Date.Year.ShouldBe(2024),
            () => result.Date.Month.ShouldBe(6),
            () => result.Date.Day.ShouldBe(15),
            // +02:00 must be normalised to UTC: 14:30+02:00 == 12:30 UTC. Asserting the hour is the whole point
            // of an offset test - without it a regression to DateTimeStyles.None (hour stays 14) goes unnoticed.
            () => result.Date.Hour.ShouldBe(12),
            () => result.Date.Kind.ShouldBe(DateTimeKind.Utc),
            () => result.Date.Minute.ShouldBe(30),
            () => result.Date.Second.ShouldBe(0)
        );
    }

    [Test]
    public void Deserialize_ColonLessOffset_ConvertsToUtc()
    {
        // The live API emits the offset WITHOUT a colon (Y-m-d\TH:i:sO, e.g. +0200). Regression guard: the old
        // TryParseExact("...zzz") silently returned DateTime.MinValue for this real-world shape.
        string json = "{\"date\":\"2024-06-15T14:30:00+0200\"}";

        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(
            () => result.Date.Hour.ShouldBe(12),
            () => result.Date.Minute.ShouldBe(30),
            () => result.Date.Kind.ShouldBe(DateTimeKind.Utc)
        );
    }

    [Test]
    public void Deserialize_ColonLessOffset_Nullable_ConvertsToUtc()
    {
        string json = "{\"date\":\"2024-06-15T14:30:00+0200\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(() => result.Date.Value.Hour.ShouldBe(12), () => result.Date.Value.Kind.ShouldBe(DateTimeKind.Utc));
    }

    [Test]
    public void Serialize_ProducesColonLessUtcOffset()
    {
        var obj = new DateTimeHolder { Date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);

        json.ShouldContain("2024-06-15T14:30:00+0000");
        json.ShouldNotContain("+00:00");
    }

    [Test]
    public void Deserialize_NullableTimezoneOffset_ConvertsToUtc()
    {
        string json = "{\"date\":\"2024-06-15T14:30:00+02:00\"}";

        NullableDateTimeHolder? result = PingenSerialisationHelper.Deserialize<NullableDateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.Date.ShouldNotBeNull();
        result.Date!.Value.ShouldSatisfyAllConditions(() => result.Date.Value.Hour.ShouldBe(12), () => result.Date.Value.Kind.ShouldBe(DateTimeKind.Utc));
    }

    [Test]
    public void RoundTrip_PreservesSecondPrecision()
    {
        var obj = new DateTimeHolder { Date = new DateTime(2024, 3, 10, 8, 45, 12, DateTimeKind.Utc) };

        string json = PingenSerialisationHelper.Serialize(obj);
        DateTimeHolder? result = PingenSerialisationHelper.Deserialize<DateTimeHolder>(json);

        result.ShouldNotBeNull();
        result!.ShouldSatisfyAllConditions(
            () => result.Date.Year.ShouldBe(2024),
            () => result.Date.Month.ShouldBe(3),
            () => result.Date.Day.ShouldBe(10),
            () => result.Date.Hour.ShouldBe(8),
            () => result.Date.Minute.ShouldBe(45),
            () => result.Date.Second.ShouldBe(12)
        );
    }

    [Test]
    public void Read_DirectInvocation_EmptyJsonString_ReturnsMinValue()
    {
        DateTime result = InvokeRead("\"\""u8.ToArray());

        result.ShouldBe(DateTime.MinValue);
    }

    [Test]
    public void Write_DirectInvocation_ProducesPingenFormat()
    {
        string output = InvokeWrite(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        output.ShouldStartWith("\"2024-01-01T00:00:00");
        output.ShouldEndWith("+0000\"");
    }

    [Test]
    public void Write_UnspecifiedKind_AssumesUtc()
    {
        string output = InvokeWrite(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Unspecified));

        output.ShouldBe("\"2024-01-01T12:00:00+0000\"");
    }

    [Test]
    public void Write_LocalKind_NormalisesToUtc()
    {
        // Noon local is safely within +-14h of the same UTC date, so only the offset shape is asserted here.
        string output = InvokeWrite(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Local));

        output.ShouldEndWith("+0000\"");
    }

    private static DateTime InvokeRead(byte[] utf8Json)
    {
        var converter = new PingenDateTimeConverter();
        var opts = new JsonSerializerOptions();
        var reader = new Utf8JsonReader(utf8Json);
        reader.Read();
        return converter.Read(ref reader, typeof(DateTime), opts);
    }

    private static string InvokeWrite(DateTime value)
    {
        var converter = new PingenDateTimeConverter();
        var opts = new JsonSerializerOptions();
        using var ms = new MemoryStream();
        // Match the production serializer's relaxed encoder so the '+' offset is written literally, not \u-escaped.
        using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
        {
            converter.Write(writer, value, opts);
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private sealed record DateTimeHolder
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; init; }
    }

    private sealed record NullableDateTimeHolder
    {
        [JsonPropertyName("date")]
        public DateTime? Date { get; init; }
    }
}

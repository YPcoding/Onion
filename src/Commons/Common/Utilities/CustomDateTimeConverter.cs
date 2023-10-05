using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Utilities;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public CustomDateTimeConverter(string format)
    {
        _format = format;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var dateTime))
        {
            return dateTime;
        }

        if (DateTime.TryParseExact(reader.GetString(), _format, null, DateTimeStyles.None, out var parsedDateTime))
        {
            return parsedDateTime;
        }

        throw new JsonException($"Unable to parse date. Expected format: {_format}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}





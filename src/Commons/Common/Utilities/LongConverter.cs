using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Utilities;

public class LongConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), out var longValue))
        {
            return longValue;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64();
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // 将 long 值转换为字符串并写入 JSON
    }
}
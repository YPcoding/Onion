using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Extensions
{
    /// <summary>
    /// 提供 JSON 相关的扩展方法。
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// 将对象序列化为 JSON 字符串，并过滤指定的敏感字段，以及去掉空数组。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="sensitiveFields">要过滤的敏感字段数组。</param>
        /// <returns>JSON 字符串。</returns>
        public static string ToJsonWithSensitiveFilter(this object obj, params string[]? sensitiveFields)
        {
            // 去掉空数组
            obj = NullifyEmptyArrays(obj);

            // 配置 JSON 序列化选项
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  // 使生成的 JSON 更易阅读
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // 忽略 null 字段
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            if (sensitiveFields!.Any())
            {
                // 添加敏感字段过滤器
                options.Converters.Add(new SensitiveFieldsConverter(sensitiveFields!));
            }

            // 执行 JSON 序列化
            var json = JsonSerializer.Serialize(obj, options);
            return json;
        }

        // 去掉空数组
        private static object NullifyEmptyArrays(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.PropertyType.IsArray)
                {
                    var value = property.GetValue(obj);

                    if (value != null && ((Array)value).Length == 0)
                    {
                        property.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }
    }

    // 自定义 JsonConverter 以过滤敏感字段
    public class SensitiveFieldsConverter : JsonConverter<object>
    {
        private readonly string[] _sensitiveFields;

        public SensitiveFieldsConverter(string[] sensitiveFields)
        {
            _sensitiveFields = sensitiveFields;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var jsonDocumentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = options.AllowTrailingCommas,
                CommentHandling = options.ReadCommentHandling,
                MaxDepth = options.MaxDepth
            };

            using (JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(value), jsonDocumentOptions))
            {
                writer.WriteStartObject();

                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    if (!_sensitiveFields.Contains(property.Name))
                    {
                        property.WriteTo(writer);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
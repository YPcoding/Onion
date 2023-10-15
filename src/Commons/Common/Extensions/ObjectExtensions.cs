using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Common.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// 检查对象是否为null
    /// </summary>
    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    /// <summary>
    /// 检查对象是否不为null
    /// </summary>
    public static bool IsNotNull(this object obj)
    {
        return obj != null;
    }

    /// <summary>
    /// 获取对象的类型名称
    /// </summary>
    public static string GetTypeName(this object obj)
    {
        return obj.GetType().FullName!;
    }

    /// <summary>
    /// 使用 System.Text.Json 将对象序列化为 JSON 字符串
    /// </summary>
    public static string ToJson(this object obj, bool isCamelCase = false)
    {
        if (!isCamelCase)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(obj, options);
        }
        else
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }

    /// <summary>
    /// 判断对象是否相等
    /// </summary>
    public static bool IsEqual(this object obj, object other)
    {
        return obj.Equals(other);
    }

    /// <summary>
    /// 克隆对象（深度克隆）
    /// </summary>
    public static T DeepClone<T>(this T source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        // 使用 System.Text.Json 进行深度克隆
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // 如果需要格式化输出，可以设置此选项
        };

        var serialized = JsonSerializer.Serialize(source, options);
        return JsonSerializer.Deserialize<T>(serialized, options)!;
    }

    /// <summary>
    /// 获取对象的哈希码
    /// </summary>
    public static int GetHash(this object obj)
    {
        return obj.GetHashCode();
    }

    /// <summary>
    /// 判断对象是否为默认值
    /// </summary>
    public static bool IsDefault<T>(this T obj)
    {
        return obj!.Equals(default(T));
    }

    /// <summary>
    /// 获取对象的字符串表示
    /// </summary>
    public static string GetString(this object obj)
    {
        return obj.ToString()!;
    }

    /// <summary>
    /// 转换对象为指定类型
    /// </summary>
    public static T ConvertTo<T>(this object obj)
    {
        return (T)Convert.ChangeType(obj, typeof(T));
    }

    /// <summary>
    /// 判断对象是否实现指定接口
    /// </summary>
    public static bool ImplementsInterface<TInterface>(this object obj)
    {
        return obj is TInterface;
    }

    /// <summary>
    /// 将对象转换为字节数组，使用 System.Text.Json 进行序列化
    /// </summary>
    public static byte[] ToByteArray(this object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        string jsonString = JsonSerializer.Serialize(obj);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    /// <summary>
    /// 判断对象是否为特定类型
    /// </summary>
    public static bool IsType<T>(this object obj)
    {
        return obj is T;
    }

    /// <summary>
    /// 安全调用方法，避免空引用异常
    /// </summary>
    public static TResult SafeInvoke<T, TResult>(this T obj, Func<T, TResult> func) where T : class
    {
        return obj != null ? func(obj) : default(TResult)!;
    }

    /// <summary>
    /// 将对象转换为Base64编码的字符串
    /// </summary>
    public static string ToBase64String(this object obj)
    {
        var bytes = Encoding.UTF8.GetBytes(obj.ToJson());
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 获取对象的哈希码，安全处理null情况
    /// </summary>
    public static int SafeGetHashCode(this object obj)
    {
        return obj?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// 获取对象的类型
    /// </summary>
    public static Type GetType(this object obj)
    {
        return obj.GetType();
    }

    /// <summary>
    /// 判断对象是否为空或空白
    /// </summary>
    public static bool IsNullOrWhiteSpace(this object obj)
    {
        if (obj == null)
        {
            return true;
        }

        if (obj is string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        return false;
    }

    /// <summary>
    /// 获取对象的字节大小
    /// </summary>
    public static int GetSizeInBytes(this object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // 使用 System.Runtime.InteropServices.Marshal.SizeOf 获取对象的字节大小
        return Marshal.SizeOf(obj);
    }

    /// <summary>
    /// 判断对象是否为特定枚举值
    /// </summary>
    public static bool IsEnumValue<TEnum>(this object obj) where TEnum : Enum
    {
        if (obj is TEnum)
        {
            TEnum value = (TEnum)obj;
            return Enum.IsDefined(typeof(TEnum), value);
        }
        return false;
    }

    /// <summary>
    /// 获取对象的属性名称和值的字典
    /// </summary>
    public static IDictionary<string, object> GetProperties(this object obj)
    {
        if (obj == null)
        {
            return null!;
        }

        return obj.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj))!;
    }

    /// <summary>
    /// 判断对象是否为默认引用值
    /// </summary>
    public static bool IsDefaultReference<T>(this T obj) where T : class
    {
        return object.ReferenceEquals(obj, null);
    }

    /// <summary>
    /// 判断对象是否为默认值或空值
    /// </summary>
    public static bool IsDefaultOrEmpty<T>(this T obj)
    {
        return obj == null || obj.Equals(default(T));
    }

    /// <summary>
    /// 将对象转换为指定类型的实例
    /// </summary>
    public static T ToInstanceOfType<T>(this object obj) where T : class
    {
        return (obj as T)!;
    }

    /// <summary>
    /// 获取对象的简短类型名称
    /// </summary>
    public static string GetShortTypeName(this object obj)
    {
        return obj.GetType().Name;
    }

    /// <summary>
    /// 判断对象是否为默认日期值
    /// </summary>
    public static bool IsDefaultDate(this object obj)
    {
        if (obj is DateTime date)
        {
            return date == default(DateTime);
        }
        return false;
    }

    /// <summary>
    /// 获取对象的HashCode，安全处理null情况
    /// </summary>
    public static int GetSafeHashCode(this object obj)
    {
        return obj?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// 获取对象的可读字符串表示
    /// </summary>
    public static string ToReadableString(this object obj)
    {
        if (obj == null)
        {
            return "null";
        }
        if (obj is string str)
        {
            return str;
        }
        return obj.ToString()!;
    }

    /// <summary>
    /// 获取对象的简短类型全名
    /// </summary>
    public static string GetShortTypeFullName(this object obj)
    {
        return obj.GetType().FullName!;
    }

    /// <summary>
    /// 判断对象是否为特定枚举值
    /// </summary>
    public static bool IsEnumValue<TEnum>(this object obj, TEnum value) where TEnum : Enum
    {
        return obj != null && Enum.Equals(obj, value);
    }

    /// <summary>
    /// 使用 System.Text.Json 将对象序列化为可读的 JSON 字符串
    /// </summary>
    public static string ToReadableJson(this object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // 创建 JsonSerializerOptions，设置 WriteIndented 为 true
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // 使用 System.Text.Json 进行序列化
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// 判断对象是否实现特定接口
    /// </summary>
    public static bool ImplementsInterface<TInterface>(this object obj, TInterface instance) where TInterface : class
    {
        return instance is TInterface;
    }

    /// <summary>
    /// 判断对象是否为空或空集合
    /// </summary>
    public static bool IsNullOrEmpty<T>(this T obj) where T : class, ICollection
    {
        return obj == null || obj.Count == 0;
    }

    /// <summary>
    /// 判断对象是否为数字类型
    /// </summary>
    public static bool IsNumeric(this object obj)
    {
        return obj is int || obj is long || obj is float || obj is double || obj is decimal;
    }

    /// <summary>
    /// 判断对象是否为字典类型
    /// </summary>
    public static bool IsDictionary(this object obj)
    {
        return obj is IDictionary;
    }

    /// <summary>
    /// 判断对象是否为可空值类型
    /// </summary>
    public static bool IsNullable(this object obj)
    {
        return obj != null && Nullable.GetUnderlyingType(obj.GetType()) != null;
    }

    /// <summary>
    /// 将对象转换为可空值类型
    /// </summary>
    public static T? ToNullable<T>(this object obj) where T : struct
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is T value)
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// 判断对象是否为IEnumerable
    /// </summary>
    public static bool IsEnumerable(this object obj)
    {
        return obj is IEnumerable;
    }

    /// <summary>
    /// 判断对象是否为指定的泛型类型
    /// </summary>
    public static bool IsGenericType<T>(this object obj)
    {
        return obj != null && obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(T);
    }

    /// <summary>
    /// 判断对象是否为指定的非泛型类型
    /// </summary>
    public static bool IsNonGenericType<T>(this object obj)
    {
        return obj != null && obj.GetType() == typeof(T);
    }

    /// <summary>
    /// 将对象转换为特定枚举类型
    /// </summary>
    public static TEnum ToEnum<TEnum>(this object obj) where TEnum : struct, Enum
    {
        if (Enum.TryParse(obj?.ToString(), true, out TEnum result))
        {
            return result;
        }
        throw new ArgumentException($"Unable to convert '{obj}' to {typeof(TEnum).Name}.");
    }

    /// <summary>
    /// 获取对象的非泛型类型
    /// </summary>
    public static Type GetNonGenericType(this object obj)
    {
        return obj?.GetType().IsGenericType == true ? obj.GetType().GetGenericTypeDefinition() : obj.GetType();
    }
}

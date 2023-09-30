using System.ComponentModel;

namespace Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举值的描述文本
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = fieldInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        if (attributes!.Length > 0)
            return attributes[0].Description;
        return value.ToString();
    }

    /// <summary>
    /// 获取枚举类型的所有值
    /// </summary>
    public static IEnumerable<TEnum> GetEnumValues<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }

    /// <summary>
    /// 将枚举值转换为字节数组
    /// </summary>
    public static byte[] ToByteArray(this Enum value)
    {
        return BitConverter.GetBytes(Convert.ToInt32(value));
    }

    /// <summary>
    /// 将枚举值转换为字符串列表
    /// </summary>
    public static List<string> ToList<TEnum>() where TEnum : Enum
    {
        return Enum.GetNames(typeof(TEnum)).ToList();
    }

    /// <summary>
    /// 检查枚举值是否包含在指定枚举类型中
    /// </summary>
    public static bool IsValidEnumValue<TEnum>(this int value) where TEnum : Enum
    {
        return Enum.IsDefined(typeof(TEnum), value);
    }

    /// <summary>
    /// 获取枚举值的下一个值（循环）
    /// </summary>
    public static TEnum GetNext<TEnum>(this TEnum value) where TEnum : Enum
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
        var index = Array.IndexOf(values, value);
        return index < values.Length - 1 ? values[index + 1] : values[0];
    }

    /// <summary>
    /// 获取枚举值的前一个值（循环）
    /// </summary>
    public static TEnum GetPrevious<TEnum>(this TEnum value) where TEnum : Enum
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
        var index = Array.IndexOf(values, value);
        return index > 0 ? values[index - 1] : values[values.Length - 1];
    }

    /// <summary>
    /// 获取枚举值的名称
    /// </summary>
    public static string GetName<TEnum>(this TEnum value) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), value)!;
    }

    /// <summary>
    /// 将字符串值解析为枚举类型，如果解析失败返回 null
    /// </summary>
    public static TEnum? ParseEnum<TEnum>(this string value) where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, out TEnum result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// 从枚举描述获取枚举值，不区分大小写
    /// </summary>
    public static TEnum GetEnumFromDescription<TEnum>(this string description) where TEnum : Enum
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));

        var type = typeof(TEnum);

        foreach (var field in type.GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (string.Equals(attribute.Description, description, StringComparison.OrdinalIgnoreCase))
                {
                    return (TEnum)field.GetValue(null)!;
                }
            }
        }

        throw new ArgumentException($"No enum value with description '{description}' found in {typeof(TEnum).Name}.");
    }
}

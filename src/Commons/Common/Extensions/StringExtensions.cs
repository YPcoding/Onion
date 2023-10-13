using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// 执行忽略大小写的字符串比较，确定两个字符串是否相等。
    /// </summary>
    /// <param name="str1">要比较的第一个字符串。</param>
    /// <param name="str2">要比较的第二个字符串。</param>
    /// <returns>如果两个字符串相等（忽略大小写），则为 true；否则为 false。</returns>
    public static bool EqualsIgnoreCase(this string str1, string str2)
    {
        return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 生成MD5哈希
    /// </summary>
    /// <param name="input">要哈希的原始字符串</param>
    /// <param name="salt">可选的盐值（默认为null）</param>
    /// <returns>MD5哈希</returns>
    public static string MDString(this string input, string? salt = null)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes;

            if (!salt.IsDefaultOrEmpty())
            {
                // 如果提供了盐值，将原始字符串和盐值拼接
                inputBytes = Encoding.UTF8.GetBytes(input + salt);
            }
            else
            {
                // 如果没有提供盐值，直接使用原始字符串
                inputBytes = Encoding.UTF8.GetBytes(input);
            }

            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // 将哈希字节数组转换为字符串表示形式
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2")); // 使用小写十六进制表示
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// 首字母转大写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // 将字符串拆分成单词
        string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // 使用 TextInfo 对象将每个单词的首字母大写
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = textInfo.ToTitleCase(words[i]);
        }

        // 将单词重新组合成字符串
        return string.Join(" ", words);
    }

    // 检查字符串是否为空或 null
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    // 检查字符串是否为空格或 null
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 在字符串中查找所有匹配的子字符串，并返回它们的列表
    /// </summary>
    public static List<string> FindAllMatches(this string input, string pattern)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Select(match => match.Value).ToList();
    }

    /// <summary>
    /// 将字符串拆分为行，并返回行的列表
    /// </summary>
    public static List<string> SplitLines(this string input)
    {
        return input.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None).ToList();
    }

    /// <summary>
    /// 移除字符串中的所有空格
    /// </summary>
    public static string RemoveSpaces(this string input)
    {
        return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    /// <summary>
    /// 反转字符串
    /// </summary>
    public static string Reverse(this string input)
    {
        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// 判断字符串是否包含指定子字符串，不区分大小写
    /// </summary>
    public static bool ContainsIgnoreCase(this string input, string value)
    {
        return input.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>
    /// 截取字符串的一部分
    /// </summary>
    public static string SubstringSafe(this string input, int startIndex, int length)
    {
        if (input.Length <= startIndex)
            return string.Empty;
        if (startIndex + length > input.Length)
            return input.Substring(startIndex);
        return input.Substring(startIndex, length);
    }

    /// <summary>
    /// 剔除字符串中的HTML标签
    /// </summary>
    public static string StripHtmlTags(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    /// <summary>
    /// 将字符串转换为小写并移除空格
    /// </summary>
    public static string ToLowerAndRemoveSpaces(this string input)
    {
        return input.ToLower().RemoveSpaces();
    }

    /// <summary>
    /// 检查字符串是否为有效URL
    /// </summary>
    public static bool IsValidUrl(this string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// 将字符串转换为Base64编码
    /// </summary>
    public static string ToBase64(this string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 将Base64编码的字符串解码为原始字符串
    /// </summary>
    public static string FromBase64(this string base64Input)
    {
        var bytes = Convert.FromBase64String(base64Input);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 将字符串中的所有空格替换为指定字符
    /// </summary>
    public static string ReplaceSpacesWith(this string input, char replacement)
    {
        return new string(input.Select(c => c == ' ' ? replacement : c).ToArray());
    }

    /// <summary>
    /// 获取字符串的字节数组表示
    /// </summary>
    public static byte[] ToByteArray(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    /// <summary>
    /// 将字符串重复指定次数
    /// </summary>
    public static string Repeat(this string input, int count)
    {
        return string.Concat(Enumerable.Repeat(input, count));
    }

    /// <summary>
    /// 将字符串分割为单词列表
    /// </summary>
    public static List<string> SplitIntoWords(this string input)
    {
        return input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    /// <summary>
    /// 判断字符串是否包含任何数字字符
    /// </summary>
    public static bool ContainsDigits(this string input)
    {
        return input.Any(char.IsDigit);
    }

    /// <summary>
    /// 将字符串转换为驼峰命名法
    /// </summary>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(' ', '-', '_');
        for (int i = 1; i < words.Length; i++)
        {
            words[i] = words[i].ToTitleCase();
        }
        return string.Join("", words);
    }

    /// <summary>
    /// 将字符串转换为帕斯卡命名法
    /// </summary>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(' ', '-', '_');
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = words[i].ToTitleCase();
        }
        return string.Join("", words);
    }

    /// <summary>
    /// 检查字符串是否为数字
    /// </summary>
    public static bool IsNumeric(this string input)
    {
        return double.TryParse(input, out _);
    }

    /// <summary>
    /// 检查字符串是否包含特定前缀
    /// </summary>
    public static bool HasPrefix(this string input, string prefix)
    {
        return input.StartsWith(prefix);
    }

    /// <summary>
    /// 检查字符串是否包含特定后缀
    /// </summary>
    public static bool HasSuffix(this string input, string suffix)
    {
        return input.EndsWith(suffix);
    }

    /// <summary>
    /// 将字符串转换为全大写
    /// </summary>
    public static string ToUpperCase(this string input)
    {
        return input.ToUpper();
    }

    /// <summary>
    /// 将字符串转换为全小写
    /// </summary>
    public static string ToLowerCase(this string input)
    {
        return input.ToLower();
    }

    /// <summary>
    /// 检查字符串是否为空或仅包含空格
    /// </summary>
    public static bool IsNullOrWhitespace(this string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }

    /// <summary>
    /// 检查字符串是否为有效的IP地址
    /// </summary>
    public static bool IsValidIpAddress(this string input)
    {
        return IPAddress.TryParse(input, out _);
    }


    /// <summary>
    /// 将字符串中的所有换行符替换为指定字符串
    /// </summary>
    public static string ReplaceNewlinesWith(this string input, string replacement)
    {
        return input.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement);
    }

    /// <summary>
    /// 将字符串转换为可安全传递的URL编码格式
    /// </summary>
    public static string ToUrlEncoded(this string input)
    {
        return Uri.EscapeDataString(input);
    }

    /// <summary>
    /// 将 JSON 字符串转换为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要转换的对象类型。</typeparam>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>转换后的对象。</returns>
    public static T FromJson<T>(this string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"JSON 解析失败：{ex.Message}", nameof(json));
        }
    }

    /// <summary>
    /// 将字符串转换为 Int64（长整数）类型。如果转换失败，返回默认值。
    /// </summary>
    /// <param name="input">要转换的字符串</param>
    /// <param name="defaultValue">转换失败时返回的默认值</param>
    /// <returns>转换后的 Int64 值或默认值</returns>
    public static long ToInt64OrDefault(this string input, long defaultValue = 0)
    {
        if (long.TryParse(input, out long result))
        {
            return result;
        }

        return defaultValue;
    }
}





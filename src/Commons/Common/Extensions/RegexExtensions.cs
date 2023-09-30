using System.Text.Json;
using System.Text.RegularExpressions;

namespace Common.Extensions;

public static class RegexExtensions
{
    /// <summary>
    /// 01. 移除字符串中的非数字字符
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string RemoveNonDigits(this string input)
    {
        return Regex.Replace(input, @"[^\d]", string.Empty);
    }

    /// <summary>
    /// 02. 移除字符串中的非字母字符
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string RemoveNonLetters(this string input)
    {
        return Regex.Replace(input, @"[^a-zA-Z]", string.Empty);
    }

    /// <summary>
    /// 03. 判断是否是有效的 IPv6 地址
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsIpv6Address(this string input)
    {
        string pattern = @"^(([0-9A-Fa-f]{1,4}:){7,7}[0-9A-Fa-f]{1,4}" +
                         @"|([0-9A-Fa-f]{1,4}:){1,7}:|([0-9A-Fa-f]{1,4}:)" +
                         @"{1,6}:[0-9A-Fa-f]{1,4}|([0-9A-Fa-f]{1,4}:){1,5}(:[0-9A-Fa-f]{1,4}){1,2}" +
                         @"|([0-9A-Fa-f]{1,4}:){1,4}(:[0-9A-Fa-f]{1,4}){1,3}|" +
                         @"([0-9A-Fa-f]{1,4}:){1,3}(:[0-9A-Fa-f]{1,4}){1,4}|" +
                         @"([0-9A-Fa-f]{1,4}:){1,2}(:[0-9A-Fa-f]{1,4}){1,5}|" +
                         @"[0-9A-Fa-f]{1,4}:((:[0-9A-Fa-f]{1,4}){1,6})|" +
                         @":((:[0-9A-Fa-f]{1,4}){1,7}|:)|fe80:(:[0-9A-Fa-f]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::" +
                         @"(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|" +
                         @"[01]?[0-9]{1,2}){0,1}[0-9]{1,2})\.){3,3}(25[0-5]|" +
                         @"(2[0-4]|[01]?[0-9]{1,2}){0,1}[0-9]{1,2})|" +
                         @"([0-9A-Fa-f]{1,4}:){1,4}:((25[0-5]|(2[0-4]|" +
                         @"[01]?[0-9]{1,2}){0,1}[0-9]{1,2})\.){3,3}" +
                         @"(25[0-5]|(2[0-4]|[01]?[0-9]{1,2}){0,1}[0-9]{1,2}))$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 04. 判断是否是有效的 UUID
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsUuid(this string input)
    {
        string pattern = @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 05. 判断是否是有效的 Windows 文件路径
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsWindowsFilePath(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^[a-zA-Z]:\\[^*\""/:<>?\\|]*$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 06. 判断是否是有效的 Unix / Linux 文件路径
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsUnixFilePath(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^/([^/]+/)*[^/]+$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 07. 判断是否包含 XML 标签
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsXmlTags(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<[^>]+>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 08. 判断是否是有效的 JSON 字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsValidJson(this string input)
    {
        try
        {
            // 尝试解析 JSON
            using (JsonDocument.Parse(input))
            {
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// 09. 获取正则表达式匹配的组值
    /// </summary>
    /// <param name="input"></param>
    /// <param name="pattern"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public static string[] GetGroupValues(this string input, string pattern, string groupName)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Cast<Match>().Select(m => m.Groups[groupName].Value).ToArray();
    }

    /// <summary>
    /// 10. 示例扩展方法
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsExample(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"example";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 11. 判断是否包含 URL
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsUrl(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"https?://\S+";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 12. 判断是否为有效的日期（YYYY-MM-DD 格式）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidDate(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^\d{4}-\d{2}-\d{2}$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 13. 判断是否为有效的邮箱地址
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 14. 判断是否为有效的手机号码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidPhoneNumber(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^1[3456789]\d{9}$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 15. 判断是否为有效的邮政编码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidPostalCode(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^\d{6}$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 16. 判断是否包含特殊字符
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsSpecialCharacters(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"[^a-zA-Z0-9\s]";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 17. 判断是否包含数字和字母
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsNumbersAndLetters(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^(?=.*[0-9])(?=.*[a-zA-Z])[a-zA-Z0-9]+$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 18. 判断是否是有效的 IPv4 地址
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsIpv4Address(this string input)
    {
        string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\." +
                         @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\." +
                         @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\." +
                         @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 19. 判断是否是有效的 URL 参数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsUrlParameter(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^[a-zA-Z0-9_.-]*$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 20. 判断是否包含 HTML 标签注释
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlComments(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<!--(.*?)-->";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 21. 判断是否是有效的 MAC 地址
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsMacAddress(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 22. 判断是否包含 HTML 链接
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlLinks(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<a\s+(?:[^>]*?\s+)?href=[""'](http[s]?://\S+)[""']";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 23. 判断是否是有效的日期和时间（YYYY-MM-DD HH:mm:ss 格式）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidDateTime(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 27. 判断是否为有效的货币金额（支持带小数点）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidCurrencyAmount(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^\d+(\.\d{1,2})?$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 28. 判断是否为有效的货币金额（支持带符号和小数点）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidCurrencyAmountWithSymbol(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^[+-]?(\d+(\.\d{1,2})?)$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 29. 判断是否为有效的颜色代码（支持 #RRGGBB 和 #AARRGGBB 格式）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidColorCode(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^#(?:[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 30. 判断是否为有效的 HTML 颜色名
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidHtmlColorName(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"^(red|green|blue|yellow|purple|orange|black|white|gray)$";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 31. 判断是否包含 HTML 图像标签
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlImages(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<img\s+[^>]*src=[""'](https?://\S+)[""']";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 32. 判断是否包含 HTML 文本框
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlTextInputs(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<input\s+[^>]*type=[""']?text[""']?";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 33. 判断是否包含 HTML 多行文本框
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlTextareaInputs(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<textarea\s+[^>]*>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 34. 判断是否包含 HTML 单选框
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlRadioInputs(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<input\s+[^>]*type=[""']?radio[""']?";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 35. 判断是否包含 HTML 多选框
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlCheckboxInputs(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<input\s+[^>]*type=[""']?checkbox[""']?";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 36. 判断是否包含 HTML 下拉框
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlSelectInputs(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<select\s+[^>]*>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 37. 判断是否包含 HTML 按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlButtons(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<button\s+[^>]*>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 38. 判断是否包含 HTML 列表项
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlListItems(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<li\s+[^>]*>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 39. 判断是否包含 HTML 表格
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlTables(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<table\s+[^>]*>";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 40. 判断是否包含 HTML 图像链接
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlImageLinks(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<a\s+[^>]*href=[""'](https?://\S+)\.(png|jpg|jpeg|gif|bmp)[""']";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 41. 判断是否包含 HTML 视频链接
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlVideoLinks(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<a\s+[^>]*href=[""'](https?://\S+)\.(mp4|avi|mov|wmv|flv)[""']";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 42. 判断是否包含 HTML 音频链接
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsHtmlAudioLinks(this string input)
    {
        // 正则表达式可根据需求调整
        string pattern = @"<a\s+[^>]*href=[""'](https?://\S+)\.(mp3|wav|wma|ogg)[""']";
        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 使用正则表达式检查输入字符串是否与模式匹配。
    /// </summary>
    /// <param name="input">要检查的输入字符串</param>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="options">正则表达式选项</param>
    /// <returns>如果匹配成功，返回 true；否则返回 false</returns>
    public static bool IsMatch(this string input, string pattern, RegexOptions options = RegexOptions.None)
    {
        return Regex.IsMatch(input, pattern, options);
    }
}

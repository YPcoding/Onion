namespace Common.Extensions;

public static class StringExtensions
{
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
}





namespace Common.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// 对集合中的每个元素执行指定的操作。
    /// </summary>
    /// <typeparam name="T">集合元素的类型</typeparam>
    /// <param name="collection">要遍历的集合</param>
    /// <param name="action">要执行的操作</param>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (var item in collection)
        {
            action(item);
        }
    }

    /// <summary>
    /// 将集合的元素使用指定的分隔符拼接成一个字符串。
    /// </summary>
    /// <typeparam name="T">集合元素的类型</typeparam>
    /// <param name="collection">要拼接的集合</param>
    /// <param name="separator">分隔符</param>
    /// <returns>拼接后的字符串</returns>
    public static string Join<T>(this IEnumerable<T> collection, string separator)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        return string.Join(separator, collection.Select(item => item!.ToString()));
    }
}

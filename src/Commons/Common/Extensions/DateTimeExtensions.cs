namespace Common.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// 获取当前日期的年份
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetYear(this DateTime dateTime)
    {
        return dateTime.Year;
    }

    /// <summary>
    /// 获取当前日期的月份
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetMonth(this DateTime dateTime)
    {
        return dateTime.Month;
    }

    /// <summary>
    /// 获取当前日期的天数
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetDay(this DateTime dateTime)
    {
        return dateTime.Day;
    }

    /// <summary>
    /// 获取当前日期的小时
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetHour(this DateTime dateTime)
    {
        return dateTime.Hour;
    }

    /// <summary>
    /// 获取当前日期的分钟
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetMinute(this DateTime dateTime)
    {
        return dateTime.Minute;
    }

    /// <summary>
    /// 获取当前日期的秒数
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetSecond(this DateTime dateTime)
    {
        return dateTime.Second;
    }

    /// <summary>
    /// 获取当前日期的毫秒数
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetMillisecond(this DateTime dateTime)
    {
        return dateTime.Millisecond;
    }

    /// <summary>
    /// 判断当前日期是否为闰年
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool IsLeapYear(this DateTime dateTime)
    {
        return DateTime.IsLeapYear(dateTime.Year);
    }

    /// <summary>
    /// 获取当前日期的季度
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetQuarter(this DateTime dateTime)
    {
        int month = dateTime.Month;
        return (int)Math.Ceiling(month / 3.0);
    }

    /// <summary>
    /// 将当前 DateTime 转换为 Unix 时间戳（秒）
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long ToUnixTimestampSeconds(this DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    /// <summary>
    /// 将当前 DateTime 转换为 Unix 时间戳（毫秒）
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    /// <summary>
    /// 从 Unix 时间戳（秒）转换为 DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime FromUnixTimestampSeconds(this long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
    }

    /// <summary>
    /// 从 Unix 时间戳（毫秒）转换为 DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime FromUnixTimestampMilliseconds(this long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
    }

    // 获取当前时间的 Unix 时间戳（秒）
    public static long CurrentUnixTimestampSeconds()
    {
        return DateTime.UtcNow.ToUnixTimestampSeconds();
    }

    // 获取当前时间的 Unix 时间戳（毫秒）
    public static long CurrentUnixTimestampMilliseconds()
    {
        return DateTime.UtcNow.ToUnixTimestampMilliseconds();
    }

    // 判断一个 Unix 时间戳是否在过去
    public static bool IsInPast(this long timestamp)
    {
        return timestamp < DateTime.UtcNow.ToUnixTimestampSeconds();
    }
    public static bool IsInFuture(this long timestamp)
    {
        return timestamp > DateTime.UtcNow.ToUnixTimestampSeconds();
    }

    /// <summary>
    /// 将 DateTimeOffset 转换为 UNIX 时间戳（秒）。
    /// </summary>
    /// <param name="dateTimeOffset">要转换的 DateTimeOffset 对象。</param>
    /// <returns>对应的 UNIX 时间戳（秒）。</returns>
    public static long ToUnixTimestampSeconds(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    /// <summary>
    /// 将 DateTimeOffset 转换为 UNIX 时间戳（毫秒）。
    /// </summary>
    /// <param name="dateTimeOffset">要转换的 DateTimeOffset 对象。</param>
    /// <returns>对应的 UNIX 时间戳（毫秒）。</returns>
    public static long ToUnixTimestampMilliseconds(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }
}

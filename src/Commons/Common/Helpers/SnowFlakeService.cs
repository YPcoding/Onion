namespace Common.Helpers;

/// <summary>
/// 雪花ID生成器服务接口
/// </summary>
public interface ISnowFlakeService
{
    /// <summary>
    /// 生成唯一的雪花ID
    /// </summary>
    /// <returns>雪花ID</returns>
    long GenerateId();
}

/// <summary>
/// 雪花ID生成器服务实现
/// </summary>
public class SnowFlakeService : ISnowFlakeService
{
    private const long Twepoch = 1288834974657L;
    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;

    private readonly long _maxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private readonly long _maxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    private long _lastTimestamp = -1L;
    private long _sequence = 0;

    private readonly object _lock = new object();

    private readonly long _workerId;
    private readonly long _datacenterId;

    /// <summary>
    /// 初始化雪花ID生成器服务
    /// </summary>
    /// <param name="workerId">工作机器ID</param>
    /// <param name="datacenterId">数据中心ID</param>
    public SnowFlakeService(long workerId, long datacenterId)
    {
        if (workerId > _maxWorkerId || workerId < 0)
            throw new ArgumentOutOfRangeException(nameof(workerId), $"Worker ID must be between 0 and {_maxWorkerId}");

        if (datacenterId > _maxDatacenterId || datacenterId < 0)
            throw new ArgumentOutOfRangeException(nameof(datacenterId), $"Datacenter ID must be between 0 and {_maxDatacenterId}");

        _workerId = workerId;
        _datacenterId = datacenterId;
    }

    /// <summary>
    /// 生成唯一的雪花ID
    /// </summary>
    /// <returns>雪花ID</returns>
    public long GenerateId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Invalid system clock!");

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                    timestamp = WaitNextTimestamp(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long WaitNextTimestamp(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}

using System.ComponentModel;

namespace Domain.Entities.Loggers;

/// <summary>
/// 日志
/// </summary>
[Description("日志")]
public class Logger : IEntity<long>
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public User? User { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public long? TimestampLong { get; set; }
    public string? Level { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}

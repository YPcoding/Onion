using System.ComponentModel;

namespace Domain.Entities.Logger;

/// <summary>
/// 日志
/// </summary>
[Description("日志")]
public class Logger
{
    public int Id { get; set; }
    public string? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Template { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public DateTime? TS { get; set; }
}

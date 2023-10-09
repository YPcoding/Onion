using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Loggers;

/// <summary>
/// 日志
/// </summary>
[Description("日志")]
public class Logger:IEntity<int>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Template { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public DateTime? TS { get; set; }
}

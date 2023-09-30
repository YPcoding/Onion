using System.ComponentModel;

namespace Domain.Entities.Logger;

/// <summary>
/// 日志
/// </summary>
[Description("日志")]
public class Logger : IEntity<long>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public long Id { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    [Description("消息")]
    public string? Message { get; set; }
    /// <summary>
    /// 消息模板
    /// </summary>
    [Description("消息模板")]
    public string? MessageTemplate { get; set; }
    /// <summary>
    /// 消息等级
    /// </summary>
    [Description("消息等级")]
    public string Level { get; set; } = default!;
    /// <summary>
    /// 发生时间
    /// </summary>
    [Description("发生时间")]
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    /// <summary>
    /// 异常
    /// </summary>
    [Description("异常")]
    public string? Exception { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    [Description("用户名")]
    public string? UserName { get; set; }
    /// <summary>
    /// 客户端IP
    /// </summary>
    [Description("客户端IP")]
    public string? ClientIP { get; set; }
    /// <summary>
    /// 客户端代理
    /// </summary>
    [Description("IP")]
    public string? ClientAgent { get; set; }
    /// <summary>
    /// 特征
    /// </summary>
    [Description("特征")]
    public string? Properties { get; set; }
    /// <summary>
    /// 日志事件
    /// </summary>
    [Description("日志事件")]
    public string? LogEvent { get; set; }
}

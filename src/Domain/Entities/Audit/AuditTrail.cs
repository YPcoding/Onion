using System.ComponentModel;

namespace Domain.Entities.Audit;

/// <summary>
/// 审计日志
/// </summary>
[Description("审计日志")]
public class AuditTrail : IEntity<long>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public long Id { get; set; }
    /// <summary>
    /// 关联用户的唯一标识
    /// </summary>
    [Description("关联用户的唯一标识")]
    public long? UserId { get; set; }
    /// <summary>
    /// 关联用户
    /// </summary>
    [Description("关联用户")]
    public virtual User? Owner { get; set; }
    /// <summary>
    /// 审计类型
    /// </summary>
    [Description("审计类型")]
    public AuditType AuditType { get; set; }
    /// <summary>
    /// 表名
    /// </summary>
    [Description("表名")]
    public string? TableName { get; set; }
    /// <summary>
    /// 审计时间
    /// </summary>
    [Description("审计时间")]
    public DateTime DateTime { get; set; }
    /// <summary>
    /// 旧值
    /// </summary>
    [Description("旧值")]
    public Dictionary<string, object?>? OldValues { get; set; }
    /// <summary>
    /// 新值
    /// </summary>
    [Description("新值")]
    public Dictionary<string, object?>? NewValues { get; set; }
    /// <summary>
    /// 受影响的列
    /// </summary>
    [Description("受影响的列")]
    public List<string>? AffectedColumns { get; set; }
    /// <summary>
    /// 主关键字
    /// </summary>
    [Description("主关键字")]
    public Dictionary<string, object> PrimaryKey { get; set; } = new();
    /// <summary>
    /// 临时属性
    /// </summary>
    [Description("临时属性")]
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    /// <summary>
    /// 具有临时属性
    /// </summary>
    [Description("具有临时属性")]
    public bool HasTemporaryProperties => TemporaryProperties.Any();
}

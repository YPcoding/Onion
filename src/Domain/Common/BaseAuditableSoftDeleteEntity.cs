using System.ComponentModel;

namespace Domain.Common;

public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    [Description("删除时间")]
    public DateTime? Deleted { get; set; }
    [Description("删除操作者")]
    public string? DeletedBy { get; set; }

}

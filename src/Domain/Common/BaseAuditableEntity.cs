
using System.ComponentModel;

namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    [Description("创建时间")]
    public virtual DateTime? Created { get; set; }

    [Description("创建者")]
    public virtual string? CreatedBy { get; set; }

    [Description("修改时间")]
    public virtual DateTime? LastModified { get; set; }

    [Description("修改者")]
    public virtual string? LastModifiedBy { get; set; }
}

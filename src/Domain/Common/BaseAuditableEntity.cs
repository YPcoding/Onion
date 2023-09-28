
using System.ComponentModel;

namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    [Description("����ʱ��")]
    public virtual DateTime? Created { get; set; }

    [Description("������")]
    public virtual string? CreatedBy { get; set; }

    [Description("�޸�ʱ��")]
    public virtual DateTime? LastModified { get; set; }

    [Description("�޸���")]
    public virtual string? LastModifiedBy { get; set; }
}

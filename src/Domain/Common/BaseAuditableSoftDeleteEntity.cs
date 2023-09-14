namespace Domain.Common;

public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

}

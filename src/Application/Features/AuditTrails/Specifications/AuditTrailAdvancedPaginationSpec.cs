using Ardalis.Specification;
using Domain.Entities.Audit;
using Masuit.Tools;

namespace Application.Features.AuditTrails.Specifications;


public class AuditTrailAdvancedPaginationSpec : Specification<AuditTrail>
{ 
    public AuditTrailAdvancedPaginationSpec(AuditTrailAdvancedFilter filter)
    {
        Query.Where(x => x.Owner!.UserName == filter.UserName, !filter.UserName!.IsNullOrEmpty())
             .Where(x => x.AuditType == filter.AuditType, filter.AuditType.HasValue)
             .Where(x => x.TableName == filter.TableName, !filter.TableName.IsNullOrEmpty());
    }
}

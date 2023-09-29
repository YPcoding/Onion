using Ardalis.Specification;
using Domain.Entities.Audit;
using Masuit.Tools;
using SharpCompress;

namespace Application.Features.AuditTrails.Specifications;

public class AuditTrailAdvancedPaginationSpec : Specification<AuditTrail>
{
    public AuditTrailAdvancedPaginationSpec(AuditTrailAdvancedFilter filter)
    {
        Query     
            .Where(x => x.TableName == filter.TableName, !filter.TableName.IsNullOrEmpty())
;    }
}
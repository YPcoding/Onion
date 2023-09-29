using Ardalis.Specification;
using Domain.Entities.Audit;

namespace Application.Features.AuditTrails.Specifications;

public class AuditTrailByIdSpec : Specification<AuditTrail>
{
    public AuditTrailByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}

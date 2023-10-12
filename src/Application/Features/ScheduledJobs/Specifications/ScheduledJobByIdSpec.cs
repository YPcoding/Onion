using Ardalis.Specification;
using Domain.Entities.Job;

namespace Application.Features.ScheduledJobs.Specifications;

public class ScheduledJobByIdSpec : Specification<ScheduledJob>
{
    public ScheduledJobByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}

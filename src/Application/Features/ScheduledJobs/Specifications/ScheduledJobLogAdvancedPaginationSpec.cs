using Application.Constants.Loggers;
using Ardalis.Specification;
using Domain.Entities.Loggers;

namespace Application.Features.ScheduledJobs.Specifications;

public class ScheduledJobLogAdvancedPaginationSpec : Specification<Logger>
{
    public ScheduledJobLogAdvancedPaginationSpec(ScheduledJobLogAdvancedFilter filter)
    {
        Query
            .Where(x => x.MessageTemplate == MessageTemplate.ScheduledJobLog)
            .Where(x=> x.Message!.Contains($"{filter.JobGroup}.{filter.JobName}"));
    }
}
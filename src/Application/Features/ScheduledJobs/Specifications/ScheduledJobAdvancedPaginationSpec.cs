using Ardalis.Specification;
using Domain.Entities.Job;


namespace Application.Features.ScheduledJobs.Specifications;

public class ScheduledJobAdvancedPaginationSpec : Specification<ScheduledJob>
{
    public ScheduledJobAdvancedPaginationSpec(ScheduledJobAdvancedFilter filter)
    {
        Query     
            .Where(x => x.JobName == filter.JobName, !filter.JobName.IsNullOrEmpty())
     
            .Where(x => x.JobGroup == filter.JobGroup, !filter.JobGroup.IsNullOrEmpty())
     
            .Where(x => x.TriggerName == filter.TriggerName, !filter.TriggerName.IsNullOrEmpty())
     
            .Where(x => x.TriggerGroup == filter.TriggerGroup, !filter.TriggerGroup.IsNullOrEmpty())
     
            .Where(x => x.CronExpression == filter.CronExpression, !filter.CronExpression.IsNullOrEmpty())
     
            .Where(x => x.Data == filter.Data, !filter.Data.IsNullOrEmpty())
     
            .Where(x => x.LastExecutionMessage == filter.LastExecutionMessage, !filter.LastExecutionMessage.IsNullOrEmpty())
;    }
}
using Ardalis.Specification;
using Domain.Entities.Loggers;


namespace Application.Features.Loggers.Specifications;

public class LoggerAdvancedPaginationSpec : Specification<Logger>
{
    public LoggerAdvancedPaginationSpec(LoggerAdvancedFilter filter)
    {
        Query
            .Where(x => x.Timestamp >= DateTimeOffset.Now.AddDays(-7), !filter.Message!.IsNullOrEmpty())
            .Where(x => x.Level == filter.Level, !filter.Level!.IsNullOrEmpty())
            .Where(x => x.Message!.Contains(filter.Message!), !filter.Message!.IsNullOrEmpty())
            .Where(x => x.Template == filter.Template)
;    }
}
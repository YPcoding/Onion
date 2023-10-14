using Application.Constants.Loggers;
using Ardalis.Specification;
using Domain.Entities.Loggers;


namespace Application.Features.Loggers.Specifications;

public class SystemLoggerAdvancedPaginationSpec : Specification<Logger>
{
    public SystemLoggerAdvancedPaginationSpec(SystemLoggerAdvancedFilter filter)
    {
        var timestampLong = DateTimeOffset.Now.AddDays(-7).ToUnixTimestampMilliseconds();
        long? startDateTime = null;
        long? endDateTime = null;
        if (filter.StartDateTime.HasValue)
        {
             startDateTime = filter.StartDateTime.Value.ToUnixTimestampMilliseconds();
        }
        if (filter.EndDateTime.HasValue)
        {
             endDateTime = filter.EndDateTime.Value.ToUnixTimestampMilliseconds();
        }
        Query
            .Where(x=> x.MessageTemplate== MessageTemplate.ActivityHistoryLog)
            .Where(x => x.Level == filter.Level, !filter.Level!.IsNullOrEmpty())
            .Where(x => x.TimestampLong >= startDateTime && x.TimestampLong <= endDateTime, filter.StartDateTime.HasValue && filter.EndDateTime.HasValue)
            .AsNoTracking()
        ;
    }
}
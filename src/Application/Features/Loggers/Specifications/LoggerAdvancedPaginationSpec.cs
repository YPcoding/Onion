using Ardalis.Specification;
using Domain.Entities.Logger;


namespace Application.Features.Loggers.Specifications;

public class LoggerAdvancedPaginationSpec : Specification<Logger>
{
    public LoggerAdvancedPaginationSpec(LoggerAdvancedFilter filter)
    {
        Query    
            .Where(x => x.Level == filter.Level, !filter.Level.IsNullOrEmpty())
     
            .Where(x => x.UserName == filter.UserName, !filter.UserName.IsNullOrEmpty())
     
            .Where(x => x.ClientIP == filter.ClientIP, !filter.ClientIP.IsNullOrEmpty())
     
            .Where(x => x.ClientAgent == filter.ClientAgent, !filter.ClientAgent.IsNullOrEmpty())
;    }
}
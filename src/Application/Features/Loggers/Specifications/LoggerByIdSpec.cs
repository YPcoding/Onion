using Ardalis.Specification;
using Domain.Entities.Loggers;

namespace Application.Features.Loggers.Specifications;

public class LoggerByIdSpec : Specification<Logger>
{
    public LoggerByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}

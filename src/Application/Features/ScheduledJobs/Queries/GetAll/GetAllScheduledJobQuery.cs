using Application.Features.ScheduledJobs.Caching;
using Application.Features.ScheduledJobs.DTOs;
using Domain.Entities.Job;
using AutoMapper.QueryableExtensions;

namespace Application.Features.ScheduledJobs.Queries.GetAll;

public class GetAllScheduledJobsQuery : IRequest<Result<IEnumerable<ScheduledJobDto>>>
{
}

public class GetAllScheduledJobsQueryHandler :
    IRequestHandler<GetAllScheduledJobsQuery, Result<IEnumerable<ScheduledJobDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllScheduledJobsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ScheduledJobDto>>> Handle(GetAllScheduledJobsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.ScheduledJobs
            .ProjectTo<ScheduledJobDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<ScheduledJobDto>>.SuccessAsync(data);
    }
}


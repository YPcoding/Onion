using Application.Features.Loggers.Caching;
using Application.Features.Loggers.DTOs;
using Domain.Entities.Loggers;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Loggers.Queries.GetAll;

[Description("获取所有日志")]
public class GetAllLoggersQuery : IRequest<Result<IEnumerable<LoggerDto>>>
{
}

public class GetAllLoggersQueryHandler :
    IRequestHandler<GetAllLoggersQuery, Result<IEnumerable<LoggerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLoggersQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<LoggerDto>>> Handle(GetAllLoggersQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Loggers
            .ProjectTo<LoggerDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<LoggerDto>>.SuccessAsync(data);
    }
}


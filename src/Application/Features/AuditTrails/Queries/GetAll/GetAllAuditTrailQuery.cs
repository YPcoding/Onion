using Application.Features.AuditTrails.Caching;
using Application.Features.AuditTrails.DTOs;
using Domain.Entities.Audit;
using AutoMapper.QueryableExtensions;

namespace Application.Features.AuditTrails.Queries.GetAll;

public class GetAllAuditTrailsQuery : IRequest<Result<IEnumerable<AuditTrailDto>>>
{
}

public class GetAllAuditTrailsQueryHandler :
    IRequestHandler<GetAllAuditTrailsQuery, Result<IEnumerable<AuditTrailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllAuditTrailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AuditTrailDto>>> Handle(GetAllAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.AuditTrails
            .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<AuditTrailDto>>.SuccessAsync(data);
    }
}


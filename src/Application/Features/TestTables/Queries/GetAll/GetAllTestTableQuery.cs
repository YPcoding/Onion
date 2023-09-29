using Application.Features.TestTables.Caching;
using Application.Features.TestTables.DTOs;
using Domain.Entities;
using AutoMapper.QueryableExtensions;

namespace Application.Features.TestTables.Queries.GetAll;

public class GetAllTestTablesQuery : IRequest<Result<IEnumerable<TestTableDto>>>
{
}

public class GetAllTestTablesQueryHandler :
    IRequestHandler<GetAllTestTablesQuery, Result<IEnumerable<TestTableDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllTestTablesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<TestTableDto>>> Handle(GetAllTestTablesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TestTables
            .ProjectTo<TestTableDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<TestTableDto>>.SuccessAsync(data);
    }
}


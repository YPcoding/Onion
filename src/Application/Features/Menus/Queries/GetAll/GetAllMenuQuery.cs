using Application.Features.Menus.Caching;
using Application.Features.Menus.DTOs;
using Domain.Entities.Identity;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Menus.Queries.GetAll;

public class GetAllMenusQuery : IRequest<Result<IEnumerable<MenuDto>>>
{
}

public class GetAllMenusQueryHandler :
    IRequestHandler<GetAllMenusQuery, Result<IEnumerable<MenuDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllMenusQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<MenuDto>>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Menus
            .ProjectTo<MenuDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<MenuDto>>.SuccessAsync(data);
    }
}


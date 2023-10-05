using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Users.Queries.GetAll;

public class GetAllUsersQuery : ICacheableRequest<Result<IEnumerable<UserDto>>>
{
    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserCacheKey.MemoryCacheEntryOptions;
}

public class GetUserQuery : ICacheableRequest<Result<UserDto>>
{
    public required long UserId { get; set; }

    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetByIdCacheKey(UserId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserCacheKey.MemoryCacheEntryOptions;
}

public class GetAllUsersQueryHandler :IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>>>, IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<UserDto>>.SuccessAsync(data);
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Users.Where(x => x.Id == request.UserId)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"角色唯一标识: {request.UserId} 未找到。");
        return await Result<UserDto>.SuccessAsync(data);
    }
}

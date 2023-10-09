using Application.Features.Roles.Caching;
using Application.Features.Roles.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Roles.Queries.GetAll;

[Description("获取所有角色数据")]
public class GetAllRolesQuery : ICacheableRequest<Result<IEnumerable<RoleDto>>>
{
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
}

public class GetRoleQuery : ICacheableRequest<Result<RoleDto>>
{
    public required long RoleId { get; set; }

    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetByIdCacheKey(RoleId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
}

public class GetAllRoleQueryByUserId : IRequest<Result<IEnumerable<RoleDto>>>
{
    public required long UserId { get; set; }
}

public class GetAllRolesQueryHandler :
    IRequestHandler<GetAllRolesQuery, Result<IEnumerable<RoleDto>>>,
    IRequestHandler<GetRoleQuery, Result<RoleDto>>,
    IRequestHandler<GetAllRoleQueryByUserId, Result<IEnumerable<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllRolesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Roles
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<RoleDto>>.SuccessAsync(data);
    }

    public async Task<Result<RoleDto>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Roles.Where(x => x.Id == request.RoleId)
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"角色唯一标识: {request.RoleId} 未找到。");
        return await Result<RoleDto>.SuccessAsync(data);
    }

    public async Task<Result<IEnumerable<RoleDto>>> Handle(GetAllRoleQueryByUserId request, CancellationToken cancellationToken)
    {
        var data = await _context.Roles.Where(ur => ur.UserRoles.Any(u => u.UserId == request.UserId))
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<RoleDto>>.SuccessAsync(data);
    }
}
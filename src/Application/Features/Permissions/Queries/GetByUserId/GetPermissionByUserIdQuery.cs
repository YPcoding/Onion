using Application.Features.Permissions.DTOs;
using Application.Features.Users.Caching;
using AutoMapper.QueryableExtensions;
using System.Text.Json.Serialization;

namespace Application.Features.Permissions.Queries.GetByUserId;

/// <summary>
/// 获取用户权限
/// </summary>
public class GetPermissionByUserIdQuery : ICacheableRequest<List<PermissionDto>>
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    public required long UserId { get; set; }
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetPermissionByUserIdCacheKey(UserId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => PermissionCacheKey.MemoryCacheEntryOptions;
}

public class GetPermissionByUserIdQueryHandler :
     IRequestHandler<GetPermissionByUserIdQuery, List<PermissionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPermissionByUserIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PermissionDto>> Handle(GetPermissionByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _context.UserRoles
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        if (!userRoles.Any()) return new List<PermissionDto>();

        var rolePermissions = await _context.RolePermissions
            .Where(x => userRoles.Select(s => s.RoleId).Contains(x.RoleId))
            .ToListAsync(cancellationToken);
        if (!rolePermissions.Any()) return new List<PermissionDto>();

        var data = await _context.Permissions
            .Where(x => rolePermissions.Select(s => s.PermissionId).Contains(x.Id))
            .ProjectTo<PermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken) ?? new List<PermissionDto>();
        return data;
    }
}

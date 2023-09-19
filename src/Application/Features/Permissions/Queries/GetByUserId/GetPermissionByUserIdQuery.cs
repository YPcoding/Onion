using Application.Features.Permissions.Caching;
using Application.Features.Permissions.DTOs;
using Domain.Entities;
using Domain.Services;

namespace Application.Features.Permissions.Queries.GetByUserId;

/// <summary>
/// 获取用户权限
/// </summary>
public class GetPermissionByUserIdQuery : ICacheableRequest<Result<IEnumerable<PermissionDto>>>
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
     IRequestHandler<GetPermissionByUserIdQuery, Result<IEnumerable<PermissionDto>>>
{
    private readonly PermissionDomainService _permissionService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPermissionByUserIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
,
        PermissionDomainService permissionService)
    {
        _context = context;
        _mapper = mapper;
        _permissionService = permissionService;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(GetPermissionByUserIdQuery request, CancellationToken cancellationToken)
    {
        var permissions =await _permissionService.GetPermissionsByUserIdAsync(request.UserId);
        return await Result<IEnumerable<PermissionDto>>
            .SuccessAsync(_mapper.Map(permissions, new List<PermissionDto>()));
    }
}

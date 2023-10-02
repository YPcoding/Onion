using Application.Features.Permissions.Caching;
using Application.Features.Permissions.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Permissions.Queries.GetAll;

/// <summary>
/// 查询全部权限
/// </summary>
public class GetAllPermissionsQuery : ICacheableRequest<Result<IEnumerable<PermissionDto>>>
{
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => PermissionCacheKey.MemoryCacheEntryOptions;

}

/// <summary>
/// 处理程序
/// </summary>
public class GetAllPermissionsQueryHandler :
     IRequestHandler<GetAllPermissionsQuery, Result<IEnumerable<PermissionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPermissionsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回全部权限数据</returns>
    public async Task<Result<IEnumerable<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions
            .ProjectTo<PermissionDto>(_mapper.ConfigurationProvider)
            .OrderBy(x=>x.Sort)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<PermissionDto>>.SuccessAsync(permission);
    }
}

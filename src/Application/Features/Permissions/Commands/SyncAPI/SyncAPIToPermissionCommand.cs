using Application.Features.Permissions.Caching;
using Application.Services;

namespace Application.Features.Permissions.Commands.SyncAPI;

/// <summary>
/// 同步API权限
/// </summary>
[Map(typeof(Permission))]
public class SyncAPIToPermissionCommand : ICacheInvalidatorRequest<Result<string>>
{
    /// <summary>
    /// 缓存Key值
    /// </summary>
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetAllCacheKey;

    /// <summary>
    /// 取消令牌源
    /// </summary>
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => PermissionCacheKey.SharedExpiryTokenSource();
}

/// <summary>
/// 处理程序
/// </summary>
public class SyncAPIToPermissionCommandHandler : IRequestHandler<SyncAPIToPermissionCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly SystemService _systemService;
    private readonly IMapper _mapper;

    public SyncAPIToPermissionCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        SystemService systemService)
    {
        _context = context;
        _mapper = mapper;
        _systemService = systemService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<string>> Handle(SyncAPIToPermissionCommand request, CancellationToken cancellationToken)
    {
        // 生成权限
        var excludePermissions = await _context.Permissions.ToListAsync();
        var generatePermissions = _systemService.GenerateMenus(excludePermissions);
        if (generatePermissions.Any())
        {
            _context.Permissions.AddRange(generatePermissions);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<string>.SuccessOrFailureAsync("同步成功", isSuccess, new string[] { "同步失败" });
        }
        return await Result<string>.SuccessAsync("没有新的权限需要添加");
    }
}
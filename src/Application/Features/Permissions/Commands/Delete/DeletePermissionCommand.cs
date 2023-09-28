using Application.Features.Permissions.Caching;
using Domain.Entities;

namespace Application.Features.Permissions.Commands.Delete;

/// <summary>
/// 删除权限
/// </summary>
public class DeletePermissionCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 权限唯一标识
    /// </summary>
    public List<long> PermissionIds { get; set; }

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
public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeletePermissionCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permissionsToDelete = await _context.Permissions
            .Where(x => request.PermissionIds.Contains(x.Id) || request.PermissionIds.Contains((long)x.SuperiorId!))
            .ToListAsync();

        if (permissionsToDelete.Any())
        {
            _context.Permissions.RemoveRange(permissionsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的权限" });
    }
}
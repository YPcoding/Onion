using Application.Features.Roles.Caching;

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Application.Features.Roles.Commands.Delete;

/// <summary>
/// 删除角色
/// </summary>
[Description("删除角色")]
public class DeleteRoleCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 角色唯一标识
    /// </summary>
    [Required(ErrorMessage = "角色唯一标识必填")]
    public List<long> RoleIds { get; set; }

    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => RoleCacheKey.SharedExpiryTokenSource(); 
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteRoleCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var rolesToDelete = await _context.Roles
            .Where(x => request.RoleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (rolesToDelete.Any())
        {
            _context.Roles.RemoveRange(rolesToDelete);

            var roleIdsToDelete = rolesToDelete.Select(r => r.Id).ToList();

            // 移除角色权限
            var roleMenusToDelete = await _context.RoleMenus
                .Where(x => roleIdsToDelete.Contains(x.RoleId))
                .ToListAsync(cancellationToken);

            if (roleMenusToDelete.Any())
            {
                _context.RoleMenus.RemoveRange(roleMenusToDelete);
            }

            // 移除用户角色
            var userRolesToDelete = await _context.UserRoles
                .Where(x => roleIdsToDelete.Contains(x.RoleId))
                .ToListAsync(cancellationToken);

            if (userRolesToDelete.Any())
            {
                _context.UserRoles.RemoveRange(userRolesToDelete);
            }

            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
        }

        return await Result<bool>.SuccessAsync(true);
    }
}
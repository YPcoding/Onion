using Application.Features.Roles.Caching;
using Masuit.Tools;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Application.Features.Roles.Commands.Delete;

/// <summary>
/// 删除角色
/// </summary>
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
    private readonly IMapper _mapper;

    public DeleteRoleCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var roles = await _context.Roles
        .Where(x => request.RoleIds.Contains(x.Id))
        .ToListAsync(cancellationToken);

        if (roles.Any()) 
        {
            _context.Roles.RemoveRange(roles);

            var rolePermissions = await _context.RolePermissions
                .Where(x => roles.Select(s=>s.Id).Contains(x.RoleId))
                .ToListAsync(cancellationToken);
            if (rolePermissions.Any()) _context.RolePermissions.RemoveRange(rolePermissions);

            var userRole = await _context.UserRoles
                .Where(x => roles.Select(s => s.Id).Contains(x.RoleId))
                .ToListAsync(cancellationToken);
            if (userRole.Any()) _context.UserRoles.RemoveRange(userRole);
        }

        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}
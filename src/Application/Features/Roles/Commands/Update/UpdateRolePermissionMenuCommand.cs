﻿using Application.Features.Roles.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRolePermissionMenuCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 角色唯一标识
    /// </summary>
    [Required(ErrorMessage = "角色唯一标识必填")]
    public long RoleId { get; set; }
    /// <summary>
    /// 权限唯一标识
    /// </summary>
    [Required(ErrorMessage = "权限唯一标识必填")]
    public List<long> PermissionIds { get; set; }
    /// <summary>
    /// 并发标记
    /// </summary>
    [Required(ErrorMessage = "并发标记必填的")]
    public string ConcurrencyStamp { get; set; }
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => RoleCacheKey.SharedExpiryTokenSource();
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionMenuCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISnowFlakeService _snowFlakeService;
    private readonly IMapper _mapper;

    public UpdateRolePermissionCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ISnowFlakeService snowFlakeService)
    {
        _context = context;
        _mapper = mapper;
        _snowFlakeService = snowFlakeService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(UpdateRolePermissionMenuCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .SingleOrDefaultAsync(x => x.Id == request.RoleId && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
             ?? throw new NotFoundException($"数据【{request.RoleId}-{request.ConcurrencyStamp}】未找到");

        var rolePermissions = await _context.RolePermissions
            .Where(x => x.RoleId == request.RoleId)
            .ToListAsync(cancellationToken);

        if (rolePermissions.Any())
        {
            _context.RolePermissions.RemoveRange(rolePermissions);
        }

        var newRolePermissions = request.PermissionIds.Select(permissionId => new RolePermission
        {
            Id = _snowFlakeService.GenerateId(),
            RoleId = request.RoleId,
            PermissionId = permissionId
        }).ToList();

        // 批量插入新的角色权限
        if (newRolePermissions.Any())
        {
            _context.RolePermissions.AddRange(newRolePermissions);
        }
        // 更新角色
        _context.Roles.Update(role);

        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}

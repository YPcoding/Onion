﻿using Application.Features.Roles.Commands.Update;

namespace Application.Features.Roles.Commands.Add;

/// <summary>
/// 修改角色校验
/// </summary>
public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateRoleCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.RoleName)
             .MaximumLength(20).WithMessage("超出最大长度20个字符")
             .NotEmpty().WithMessage("角色名不能为空")
             .MustAsync(BeUniqueRoleName).WithMessage($"角色名已存在");

        RuleFor(v => v.Description)
             .MaximumLength(50).WithMessage("超出最大长度50个字符")
             .NotEmpty().WithMessage("角色描述不能为空");

        RuleFor(v => v.PermissionIds)
              .MustAsync(BeExistPermissions).WithMessage($"权限不存在");
    }

    /// <summary>
    /// 校验角色名是否唯一
    /// </summary>
    /// <param name="roleName">角色名称</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeUniqueRoleName(UpdateRoleCommand command, string roleName, CancellationToken cancellationToken)
    {
       var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);
        if (role == null || role.Id == command.RoleId)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 校验权限是否存在
    /// </summary>
    /// <param name="permissionIds">权限唯一标识</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeExistPermissions(List<long>? permissionIds, CancellationToken cancellationToken)
    {
        if (permissionIds != null && permissionIds.Count > 0)
        {
            permissionIds = permissionIds.Distinct().ToList();
            var permissions =await _context.Permissions.Where(x=> permissionIds.Contains(x.Id)).ToListAsync(cancellationToken);
            return permissions?.Count == permissionIds.Count;
        }

        return true;
    }
}

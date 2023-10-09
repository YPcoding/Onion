using Application.Features.Roles.Commands.Update;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Features.Roles.Commands.Add;

/// <summary>
/// 添加角色校验
/// </summary>
public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    private readonly IApplicationDbContext _context;
    public AddRoleCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.RoleName)
             .MaximumLength(20).WithMessage("超出最大长度20个字符")
             .NotEmpty().WithMessage("角色名不能为空")
             .MustAsync(BeUniqueRoleName).WithMessage($"角色名已存在"); ;

        RuleFor(v => v.Description)
             .MaximumLength(50).WithMessage("超出最大长度50个字符")
             .NotEmpty().WithMessage("角色描述不能为空");

        RuleFor(v => v.RoleCode)
             .MaximumLength(50).WithMessage("超出最大长度50个字符")
             .NotEmpty().WithMessage("角色标识不能为空")
             .MustAsync(BeUniqueRoleCode).WithMessage($"角色标识已存在");

    }

    /// <summary>
    /// 校验角色名是否唯一
    /// </summary>
    /// <param name="roleName">角色名称</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeUniqueRoleName(string roleName, CancellationToken cancellationToken)
    {
        return !(await _context.Roles.AnyAsync(x => x.RoleName == roleName, cancellationToken));
    }

    /// <summary>
    /// 校验角色标识是否唯一
    /// </summary>
    ///  <param name="command">请求参数</param>
    /// <param name="roleCode">角色名称</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeUniqueRoleCode(AddRoleCommand command, string roleCode, CancellationToken cancellationToken)
    {
        bool isAlphabetOnly = Regex.IsMatch(roleCode, "^[a-zA-Z]+$");
        if (!isAlphabetOnly) return false;

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleCode == roleCode, cancellationToken);
        if (role == null) return true;

        return false;
    }
}

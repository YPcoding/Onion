namespace Application.Features.Users.Commands.Add;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    private readonly IApplicationDbContext _context;
    public AddUserCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.UserName)
             .MaximumLength(20).WithMessage("超出最大长度20个字符")
             .NotEmpty().WithMessage("用户名不能为空")
             .MustAsync(BeUniqueUserName).WithMessage($"用户名已存在");

        RuleFor(v => v.Password)
            .MinimumLength(6).WithMessage($"最小长度不能低于6个字符")
            .MaximumLength(16).WithMessage($"最大长度不能超过16个字符")
            .Must(BeValidPasswordCanNotBeChinese).WithMessage($"不能输入中文");

        RuleFor(v => v.ConfirmPassword)
            .Must(BeValidConfirmPassword).WithMessage($"两次密码不一致");

        RuleFor(v => v.RoleIds)
              .MustAsync(BeExistRoles).WithMessage($"角色不存在");
    }

    /// <summary>
    /// 校验密码是否包含中文，以及密码长度6-16位
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns></returns>
    private bool BeValidPasswordCanNotBeChinese(string password)
    {
        return password.IsMatch("^[^\u4e00-\u9fa5 ]{6,16}$");
    }

    /// <summary>
    /// 校验用户名是否唯一
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        return !(await _context.Users.AnyAsync(x => x.UserName == userName, cancellationToken));
    }

    /// <summary>
    /// 比较两个密码是否一致
    /// </summary>
    /// <param name="command">密码</param>
    /// <param name="confirmPassword">确认密码</param>
    /// <returns></returns>
    private bool BeValidConfirmPassword(AddUserCommand command, string confirmPassword)
    {
        return command.Password == confirmPassword;
    }

    /// <summary>
    /// 校验角色是否存在
    /// </summary>
    /// <param name="roleIds">角色唯一标识</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns></returns>
    private async Task<bool> BeExistRoles(List<long>? roleIds, CancellationToken cancellationToken)
    {
        if (roleIds != null && roleIds.Count > 0)
        {
            roleIds = roleIds.Distinct().ToList();
            var roles = await _context.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync(cancellationToken);
            return roles?.Count == roleIds.Count;
        }

        return true;
    }
}

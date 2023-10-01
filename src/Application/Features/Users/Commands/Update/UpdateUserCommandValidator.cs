using System.Text.RegularExpressions;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateUserCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.Email)
             .Must(BeValidEmail!).WithMessage($"邮箱格式错误");

        RuleFor(v => v.PhoneNumber)
             .Must(BeValidPhoneNumber!).WithMessage($"号码格式错误");

        //RuleFor(v => v.ProfilePictureDataUrl)
        //     .Must(BeValidProfilePictureDataUrl!).WithMessage($"链接格式错误");

        RuleFor(v => v.RoleIds)
             .MustAsync(BeExistRoles).WithMessage($"角色不存在");

        RuleFor(v => v.SuperiorId)
             .MustAsync(BeExistSuperiorId).WithMessage($"上级不存在");

        RuleFor(v=>v.SuperiorId)
            .MustAsync(BeValidSuperiorIdWithUserId).WithMessage($"上级不能是自己本身");
    }

    /// <summary>
    /// 校验邮箱
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns></returns>
    private bool BeValidEmail(string email)
    {
        if (email.IsNullOrEmpty()) 
        {
            return true;
        }
        else 
        {
            return email.IsValidEmail();
        }
    }

    /// <summary>
    /// 校验手机号码
    /// </summary>
    /// <param name="phoneNumber">手机号码</param>
    /// <returns>返回布尔值</returns>
    private bool BeValidPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.IsNullOrEmpty())
        {
            return true;
        }
        else
        {
            return phoneNumber.IsValidPhoneNumber();
        }
    }

    ///// <summary>
    ///// 校验头像
    ///// </summary>
    ///// <param name="profilePictureDataUrl">头像链接地址</param>
    ///// <returns></returns>
    //private bool BeValidProfilePictureDataUrl(string profilePictureDataUrl)
    //{
    //    if (profilePictureDataUrl.IsNullOrEmpty())
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        bool isUrl = profilePictureDataUrl.MatchUrl();
    //        return isUrl;
    //    }
    //}

    /// <summary>
    /// 校验角色是否存在
    /// </summary>
    /// <param name="roleIds">角色唯一标识</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回布尔值</returns>
    private async Task<bool> BeExistRoles(List<long>? roleIds, CancellationToken cancellationToken)
    {
        if (roleIds!.Any())
        {
            roleIds = roleIds!.Distinct().ToList();
            var roles = await _context.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync(cancellationToken);
            return roles?.Count == roleIds.Count;
        }

        return true;
    }

    /// <summary>
    /// 上级唯一标识是否存在
    /// </summary>
    /// <param name="superiorId">上级唯一标识</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回布尔值</returns>
    private async Task<bool> BeExistSuperiorId(long? superiorId, CancellationToken cancellationToken)
    {
        if (superiorId.HasValue)
        {
            return await _context.Users.Where(x => x.Id == superiorId).AnyAsync(cancellationToken);
        }
        return true;
    }

    /// <summary>
    /// 校验用户唯一标识与上级唯一标识是否一至
    /// </summary>
    /// <param name="command">修改参数</param>
    /// <param name="superiorId">上级唯一标识</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回布尔值</returns>
    private async Task<bool> BeValidSuperiorIdWithUserId(UpdateUserCommand command, long? superiorId, CancellationToken cancellationToken)
    {
        return await Task.Run(() => 
        {
            return command.UserId != superiorId;
        });
    }
}
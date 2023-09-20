using Application.Features.Auth.DTOs;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Application.Features.Auth.Commands;

/// <summary>
/// 通过用户名和密码登录
/// </summary>
public class LoginByUserNameAndPasswordCommand : IRequest<Result<LoginResultDto>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名是必填的")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码是必填的")]
    public string Password { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class LoginByUserNameAndPasswordCommandHandler : IRequestHandler<LoginByUserNameAndPasswordCommand, Result<LoginResultDto>>
{
    private readonly UserDomainService _userDomainService;
    private readonly ITokenService _tokenService;
    private readonly IOptions<JwtSettings> _optJwtSettings;
    private readonly IRoleRepository _roleRepository;

    public LoginByUserNameAndPasswordCommandHandler(
        UserDomainService userDomainService,
        ITokenService tokenService,
        IOptions<JwtSettings> optJwtSettings,
        IRoleRepository roleRepository)
    {
        _userDomainService = userDomainService;
        _tokenService = tokenService;
        _optJwtSettings = optJwtSettings;
        _roleRepository = roleRepository;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回Token</returns>
    public async Task<Result<LoginResultDto>> Handle(LoginByUserNameAndPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userDomainService.LoginByUserNameAndPasswordAsync(request.UserName, request.Password);
        if (user == null) return await Result<LoginResultDto>.FailureAsync(new string[] { "登录失败，用户名或密码错误" });

        var claims = await _tokenService.CreateClaimsAsync(user.Id, user.UserName!);
        string token = await _tokenService.BuildAsync(claims, _optJwtSettings.Value);
        var roles = await _roleRepository.GetUserRolesAsync(x=>x.UserRoles.Any(ur=>ur.UserId== user!.Id));
        var result = new LoginResultDto
        {
            Username = request.UserName,
            Roles = roles?.Select(s => s.RoleName.ToLower())?.ToArray() ?? Array.Empty<string>(),
            AccessToken = token,
            RefreshToken = token,
            Expires = DateTime.Now.AddSeconds(_optJwtSettings.Value.ExpireSeconds).ToString("yyyy/MM/dd HH:mm:ss")
        };

        return await Result<LoginResultDto>.SuccessAsync(result);
    }
}

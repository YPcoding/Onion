using Domain.Services;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Application.Features.Auth.Commands;

/// <summary>
/// 通过用户名和密码登录
/// </summary>
public class LoginByUserNameAndPasswordCommand : IRequest<Result<string>>
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
public class LoginByUserNameAndPasswordCommandHandler : IRequestHandler<LoginByUserNameAndPasswordCommand, Result<string>>
{
    private readonly UserDomainService _userDomainService;
    private readonly ITokenService _tokenService;
    private readonly IOptions<JwtSettings> _optJwtSettings;

    public LoginByUserNameAndPasswordCommandHandler(
        UserDomainService userDomainService,
        ITokenService tokenService,
        IOptions<JwtSettings> optJwtSettings)
    {
        _userDomainService = userDomainService;
        _tokenService = tokenService;
        _optJwtSettings = optJwtSettings;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回Token</returns>
    public async Task<Result<string>> Handle(LoginByUserNameAndPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userDomainService.LoginByUserNameAndPasswordAsync(request.UserName, request.Password);
        if (user == null) return await Result<string>.FailureAsync(new string[] { "登录失败，用户名或密码错误" });
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString()!),
            new Claim(ClaimTypes.Name,user?.UserName!)
        };
        string token = await _tokenService.BuildTokenAsync(claims, _optJwtSettings.Value);
        return await Result<string>.SuccessAsync(token);
    }
}

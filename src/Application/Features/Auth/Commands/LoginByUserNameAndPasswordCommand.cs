using Application.Common.Interfaces.Caching;
using Application.Features.Auth.DTOs;
using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Commands;

/// <summary>
/// 通过用户名和密码登录
/// </summary>
[Description("登录系统")]
public class LoginByUserNameAndPasswordCommand : ICacheInvalidatorRequest<Result<LoginResultDto>>
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

    /// <summary>
    /// 缓存Key值
    /// </summary>
    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetAllCacheKey;

    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => UserCacheKey.SharedExpiryTokenSource();
}

/// <summary>
/// 处理程序
/// </summary>
public class LoginByUserNameAndPasswordCommandHandler : IRequestHandler<LoginByUserNameAndPasswordCommand, Result<LoginResultDto>>
{
    private readonly UserDomainService _userDomainService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IOptions<JwtSettings> _optJwtSettings;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public LoginByUserNameAndPasswordCommandHandler(
        UserDomainService userDomainService,
        ITokenService tokenService,
        IOptions<JwtSettings> optJwtSettings,
        IRoleRepository roleRepository,
        IMapper mapper,
        IUserRepository userRepository)
    {
        _userDomainService = userDomainService;
        _tokenService = tokenService;
        _optJwtSettings = optJwtSettings;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _userRepository = userRepository;
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

        if (user == null)
        {
            return await Result<LoginResultDto>.FailureAsync(new string[] { "登录失败，用户名或密码错误" });
        }

        if (user.LockoutEnabled)
        {
            if (user.LockoutEnd > DateTime.Now)
            {
                return await Result<LoginResultDto>.FailureAsync(new string[] { "账号已被锁定", $"于{user.LockoutEnd.ToString()}解锁" });
            }

            user.IsUnLock();
        }
        user.AccessFailedCount = 0;
        await _userRepository.UpdateAsync(user);

        var claims = await _tokenService.CreateClaimsAsync(user.Id, user.UserName!, user.ProfilePictureDataUrl);
        string token = await _tokenService.BuildAsync(claims, _optJwtSettings.Value);

        var roles = await _roleRepository.GetUserRolesAsync(x => x.UserRoles.Any(ur => ur.UserId == user.Id));

        var result = new LoginResultDto
        {
            Username = request.UserName,
            UserInfo = _mapper.Map<UserDto>(user),
            Roles = roles?.Select(s => s.RoleName)?.ToArray() ?? Array.Empty<string>(),
            AccessToken = token,
            RefreshToken = token,
            Expires = DateTime.Now.AddSeconds(_optJwtSettings.Value.ExpireSeconds).ToString("yyyy/MM/dd HH:mm:ss")
        };

        return await Result<LoginResultDto>.SuccessAsync(result);
    }
}

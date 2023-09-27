using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Constants.ClaimTypes;
using Application.Features.Auth.Commands;
using Application.Features.Auth.DTOs;
using Application.Features.Permissions.DTOs;
using Application.Features.Permissions.Queries.GetByUserId;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 授权管理
    /// </summary>
    public class AuthController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IOptions<JwtSettings> _optJwtSettings;
        private readonly ICurrentUserService  _currentUserService;

        public AuthController(
            ITokenService tokenService,
            IOptions<JwtSettings> optJwtSettings,
            ICurrentUserService currentUserService)
        {
            _tokenService = tokenService;
            _optJwtSettings = optJwtSettings;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("LoginByUserNameAndPassword")]
        [AllowAnonymous]

        public async Task<Result<LoginResultDto>> LoginWithUserNameAndPassword(LoginByUserNameAndPasswordCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpPost("RefreshToken/{refreshToken}")]
        [AllowAnonymous]
        public async Task<Result<RefreshTokenDto>> RefreshToken(string refreshToken)
        {
            var jwtSecurityToken = await _tokenService.DecodeAsync(refreshToken);
            var claims = jwtSecurityToken?.Claims?.ToArray();
            if (!claims!.Any())
                throw new ArgumentException("无法解析token");

            var refreshExpires = claims!.FirstOrDefault(a => a.Type == ApplicationClaimTypes.RefreshExpires)?.Value;
            if (refreshExpires.IsNullOrEmpty() || refreshExpires.ToInt64() <= DateTime.Now.GetTotalSeconds())
                throw new Exception("登录信息已过期");

            //验签
            var securityKey = _optJwtSettings.Value.SecurityKey;
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256);
            var input = jwtSecurityToken!.RawHeader + "." + jwtSecurityToken.RawPayload;
            if (jwtSecurityToken.RawSignature != JwtTokenUtilities.CreateEncodedSignature(input, signingCredentials))
            {
                throw new Exception("验签失败");
            }

            var userId = claims![0].Value.ToInt64();
            var userName = claims[1].Value;

            var newClaims = await _tokenService.CreateClaimsAsync(userId, userName);
            var newToken = await _tokenService.BuildAsync(newClaims, _optJwtSettings.Value);

            var data = new RefreshTokenDto()
            {
                AccessToken = newToken,
                RefreshToken = newToken,
                Expires = DateTime.Now.AddSeconds(_optJwtSettings.Value.ExpireSeconds)
                                      .ToString("yyyy/MM/dd HH:mm:ss")
            };

            return await Result<RefreshTokenDto>.SuccessAsync(data);
        }

        /// <summary>
        /// 获取登录者权限路由
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoginerPermissionRouter")]

        public async Task<Result<IEnumerable<PermissionRouterDto>>> GetLoginerPermissionRouter()
        {
            return await Mediator.Send(new GetLoginerPermissionRouterQuery()
            {
                UserId = _currentUserService.CurrentUserId
            });
        }
    }
}

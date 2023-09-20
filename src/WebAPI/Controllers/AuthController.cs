using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Constants.ClaimTypes;
using Application.Features.Auth.Commands;
using Application.Features.Auth.DTOs;
using Domain.Entities.Identity;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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

        public AuthController(
            ITokenService tokenService, 
            IOptions<JwtSettings> optJwtSettings)
        {
            _tokenService = tokenService;
            _optJwtSettings = optJwtSettings;
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
        /// <returns></returns>
        [HttpPost("RefreshToken/{token}")]
        [AllowAnonymous]

        public async Task<Result<string>> RefreshToken(string token)
        {
            var jwtSecurityToken = await _tokenService.DecodeAsync(token);
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

            return await Result<string>.SuccessAsync(newToken);
        }
    }
}

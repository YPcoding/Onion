using Application.Common.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    /// <summary>
    /// 生成Token服务
    /// </summary>
    public class TokenService : ITokenService
    {
        public async Task<string> BuildTokenAsync(IEnumerable<Claim> claims, JwtSettings options)
        {
            return await Task.Run(() =>
            {
                TimeSpan ExpiryDuration = TimeSpan.FromSeconds(options.ExpireSeconds);
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                var tokenDescriptor = new JwtSecurityToken(
                    options.Issuer,
                    options.Audience,
                    claims,
                    expires: DateTime.Now.Add(ExpiryDuration),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            });
        }
    }
}

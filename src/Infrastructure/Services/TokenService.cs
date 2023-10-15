using Application.Common.Configurations;
using Application.Constants.ClaimTypes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public async Task<string> BuildAsync(IEnumerable<Claim> claims, JwtSettings options)
        {
            return await Task.Run(() =>
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var timestamp = DateTime.Now.AddMinutes(options.ExpireSeconds + options.RefreshExpiresSeconds).ToUnixTimestampSeconds().ToString();
                claims = claims.Append(new Claim(ApplicationClaimTypes.RefreshExpires, timestamp)).ToArray();

                var token = new JwtSecurityToken(
                    issuer: options.Issuer,
                    audience: options.Audience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddMinutes(options.ExpireSeconds),
                    signingCredentials: signingCredentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }

        public async Task<IEnumerable<Claim>> CreateClaimsAsync(long userId, string userName, string? profilePictureDataUrl = null)
        {
            return await Task.Run(() =>
            {
                List<Claim> claims = new()
                {
                  new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                  new Claim(ClaimTypes.Name,userName),
                  new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, profilePictureDataUrl??"")
                 };
                return claims;
            });
        }

        public async Task<JwtSecurityToken> DecodeAsync(string jwtToken)
        {
            return await Task.Run(() =>
            {
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
                return jwtSecurityToken;
            });
        }
    }
}

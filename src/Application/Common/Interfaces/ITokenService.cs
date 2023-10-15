using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Common.Interfaces;

/// <summary>
/// 生成Token服务
/// </summary>
public interface ITokenService : IScopedDependency
{
    /// <summary>
    /// 生成Token
    /// </summary>
    /// <param name="claims">Token信息</param>
    /// <param name="options">生成Token配置信息</param>
    /// <returns>返回Tokne字符串</returns>
    Task<string> BuildAsync(IEnumerable<Claim> claims, JwtSettings options);

    /// <summary>
    /// 解密Token
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    Task<JwtSecurityToken> DecodeAsync(string jwtToken);

    /// <summary>
    /// 创建Claim
    /// </summary>
    /// <param name="userId">用户唯一标识</param>
    /// <param name="userName">用户名</param>
    /// <param name="profilePictureDataUrl">头像</param>
    /// <returns></returns>
    Task<IEnumerable<Claim>> CreateClaimsAsync(long userId, string userName, string? profilePictureDataUrl = null);
}

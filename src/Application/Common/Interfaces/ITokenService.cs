using System.Security.Claims;

namespace Application.Common.Interfaces;

/// <summary>
/// 生成Token服务
/// </summary>
public interface ITokenService : IScopedDependency
{
    Task<string> BuildTokenAsync(IEnumerable<Claim> claims, JwtSettings options);
}

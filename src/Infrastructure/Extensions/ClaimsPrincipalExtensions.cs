using Application.Constants.ClaimTypes;
using System.Security.Claims;

namespace Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    public static string? GetProvider(this ClaimsPrincipal claimsPrincipal)
      => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider);
    public static string? GetDisplayName(this ClaimsPrincipal claimsPrincipal)
         => claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    public static string? GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);
    public static string? GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName);
    public static string? GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorId);
    public static string? GetTenantName(this ClaimsPrincipal claimsPrincipal)
         => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantName);
    public static string? GetTenantId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.TenantId);
    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
       => Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    public static string? GetAssignRoles(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.AssignedRoles);
    public static string[]? GetRoles(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
    public static string? GetRefreshExpires(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.RefreshExpires);
}

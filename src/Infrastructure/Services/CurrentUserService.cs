using Infrastructure.Extensions;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor
       )
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string? UserId => _httpContextAccessor.HttpContext?.User.GetUserId();
    public string? Email => _httpContextAccessor.HttpContext?.User.GetEmail();
    public string? UserName => _httpContextAccessor.HttpContext?.User.GetUserName();
    public string? TenantId => _httpContextAccessor.HttpContext?.User.GetTenantId();
    public string? TenantName => _httpContextAccessor.HttpContext?.User.GetTenantName();
    public string? DisplayName => _httpContextAccessor.HttpContext?.User.GetDisplayName();
    public string? SuperiorId => _httpContextAccessor.HttpContext?.User.GetSuperiorId();
    public string? SuperiorName => _httpContextAccessor.HttpContext?.User.GetSuperiorName();
    public string? ProfilePictureDataUrl => _httpContextAccessor.HttpContext?.User.GetProfilePictureDataUrl();
    public string[]? AssignRoles
    {
        get
        {
            var str = _httpContextAccessor.HttpContext?.User.GetAssignRoles() ?? string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public long CurrentUserId
    {
        get
        {
            long userId;
            if (long.TryParse(UserId, out userId))
            {
                return userId;
            }
            return 0;
        }
    }
}

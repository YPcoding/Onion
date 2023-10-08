using Microsoft.Extensions.Primitives;

namespace Application.Features.Roles.Caching;

public static class RoleCacheKey
{
    public const string GetAllCacheKey = "all-Roles";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetByIdCacheKey(long id)
    {
        return $"GetRoleById,{id}";
    }

    public static string GetPermissionByIdCacheKey(long id) 
    {
        return $"GetRolePermissionById,{id}";
    }

    public static string GetMenuByIdCacheKey(long id)
    {
        return $"GetRoleMenuById,{id}";
    }

    public static string GetByUserIdCacheKey(long id)
    {
        return $"GetRoleByUserId,{id}";
    }

    static RoleCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"RolesWithPaginationQuery,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested) _tokenSource = new CancellationTokenSource(RefreshInterval);
        return _tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource().Cancel();
    }
}
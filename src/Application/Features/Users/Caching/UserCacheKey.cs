using Microsoft.Extensions.Primitives;

namespace Application.Features.Users.Caching;

public static class UserCacheKey
{
    public const string GetAllCacheKey = "all-Users";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetUserByIdCacheKey(int id)
    {
        return $"GetUserById,{id}";
    }

    static UserCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"UsersWithPaginationQuery,{parameters}";
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

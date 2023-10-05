using Microsoft.Extensions.Primitives;
using Domain.Entities.Settings;
namespace Application.Features.UserProfileSettings.Caching;

public static class UserProfileSettingCacheKey
{
    public const string GetAllCacheKey = "all-UserProfileSettings";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetByIdCacheKey(long id)
    {
        return $"GetUserProfileSettingsById,{id}";
    }

    public static string GetByUserIdCacheKey(long userId)
    {
        return $"GetUserProfileSettingsByUserId,{userId}";
    }

    static UserProfileSettingCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"UserProfileSettingsWithPaginationQuery,{parameters}";
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

using Microsoft.Extensions.Primitives;
using Domain.Entities.Audit;
namespace Application.Features.AuditTrails.Caching;

public static class AuditTrailCacheKey
{
    public const string GetAllCacheKey = "all-AuditTrails";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    private static CancellationTokenSource _tokenSource;

    public static string GetByIdCacheKey(long id)
    {
        return $"GetAuditTrailById,{id}";
    }

    static AuditTrailCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"AuditTrailsWithPaginationQuery,{parameters}";
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

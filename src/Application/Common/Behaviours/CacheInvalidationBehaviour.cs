using LazyCache;

namespace Application.Common.Behaviours;

/// <summary>
/// 删除缓存
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CacheInvalidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest :  ICacheInvalidatorRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger;

    public CacheInvalidationBehaviour(
        IAppCache cache,
        ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
        )
    {
        _cache = cache;
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogTrace("{Name} 缓存过期时间 {@Request}", nameof(request), request);
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            _cache.Remove(request.CacheKey);
        }
        request.SharedExpiryTokenSource?.Cancel();
        return response;
    }
}

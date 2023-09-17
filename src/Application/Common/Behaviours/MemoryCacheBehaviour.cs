using LazyCache;

namespace Application.Common.Behaviours;

/// <summary>
/// Ìí¼Ó»º´æ
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class MemoryCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<MemoryCacheBehaviour<TRequest, TResponse>> _logger;

    public MemoryCacheBehaviour(
        IAppCache cache,
        ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger
        )
    {
        _cache = cache;
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogTrace("{Name} ÕýÔÚ»º´æ {@Request}", nameof(request),request);
        var response = await _cache.GetOrAddAsync(
            request.CacheKey,
            async () =>
            await next(),
            request.Options).ConfigureAwait(false); 
     
        return response;
    }
}

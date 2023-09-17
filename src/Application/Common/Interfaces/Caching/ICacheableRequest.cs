using Microsoft.Extensions.Caching.Memory;

namespace Application.Common.Interfaces.Caching;

/// <summary>
/// ���û���
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get => String.Empty; }
    MemoryCacheEntryOptions? Options { get; }
}

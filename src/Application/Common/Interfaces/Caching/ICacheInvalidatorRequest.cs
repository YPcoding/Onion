namespace Application.Common.Interfaces.Caching;

/// <summary>
/// ª∫¥Ê ß–ß
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get => String.Empty; } 
    CancellationTokenSource? SharedExpiryTokenSource { get; }
}

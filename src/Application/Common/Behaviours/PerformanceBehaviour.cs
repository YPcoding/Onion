using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Application.Common.Behaviours;

/// <summary>
/// MediatR 管道行为，用于性能监控。
/// </summary>
/// <typeparam name="TRequest">请求类型。</typeparam>
/// <typeparam name="TResponse">响应类型。</typeparam>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    /// <summary>
    /// 初始化一个 <see cref="PerformanceBehaviour{TRequest, TResponse}"/> 实例。
    /// </summary>
    /// <param name="logger">日志记录器。</param>
    public PerformanceBehaviour(
        ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    /// <summary>
    /// 处理请求并进行性能监控。
    /// </summary>
    /// <param name="request">要处理的请求。</param>
    /// <param name="next">请求处理委托。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理后的响应。</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next().ConfigureAwait(false);

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        if (elapsedMilliseconds >500)
        {
            _logger.LogWarning($"耗时请求：{elapsedMilliseconds}毫秒");
        }
       
        return response;
    }
}
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
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISnowFlakeService _snowFlakeService;

    /// <summary>
    /// 初始化一个 <see cref="PerformanceBehaviour{TRequest, TResponse}"/> 实例。
    /// </summary>
    /// <param name="logger">日志记录器。</param>
    /// <param name="currentUserService">当前用户服务。</param>
    /// <param name="httpContextAccessor"></param>
    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        ISnowFlakeService snowFlakeService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _snowFlakeService = snowFlakeService;
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

        var response = await next().ConfigureAwait(false); ;

        _timer.Stop();


        var httpContext = _httpContextAccessor.HttpContext;
        var id = _snowFlakeService.GenerateId();
        var loggerName = typeof(TRequest).Name;
        var requestPath = httpContext?.Request.Path!;
        var requestName = typeof(TRequest).Name;
        var requestMethod = httpContext?.Request.Method!;
        var userName = _currentUserService.UserName ?? "匿名";
        var clientIP = httpContext?.Connection?.RemoteIpAddress?.ToString()!;
        var statusCode = httpContext?.Response.StatusCode;
        var loggerTime = DateTime.Now.ToString("G");
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning("{ID},{LoggerName},{RequestPath},{RequestName},{RequestMethod},{UserName},{ClientIP},{ResponseStatusCode},{LoggerTime},{ElapsedMilliseconds}",
                                id, loggerName, requestPath, requestName, requestMethod, userName, clientIP, statusCode, loggerTime, elapsedMilliseconds);
        }
        else 
        {
            _logger.LogInformation("{ID},{LoggerName},{RequestPath},{RequestName},{RequestMethod},{UserName},{ClientIP},{ResponseStatusCode},{LoggerTime},{ElapsedMilliseconds}",
                                id, loggerName, requestPath, requestName, requestMethod, userName, clientIP, statusCode, loggerTime, elapsedMilliseconds);
        }   

        return response;
    }
}
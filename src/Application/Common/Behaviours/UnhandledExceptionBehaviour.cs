using Application.Constants.Loggers;
using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ISnowFlakeService _snowFlakeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnhandledExceptionBehaviour(
        ILogger<TRequest> logger, 
        ISnowFlakeService snowFlakeService, 
        ICurrentUserService currentUserService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _snowFlakeService = snowFlakeService;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isException = false;
        TResponse response = default!;
        try
        {
            _timer.Start();
            response = await next().ConfigureAwait(false);
            _timer.Stop();

            return response;
        }
        catch (Exception ex)
        {
            isException = true;
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService.UserName;
            _logger.LogError(ex, "{Name}: {Exception} with {@Request} by {@UserName}", requestName, ex.Message, request, userName);
            throw;
        }
        finally 
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var id = _snowFlakeService.GenerateId();
                var loggerName = typeof(TRequest).GetDescription();
                var userAgent = httpContext.Request.Headers["User-Agent"][0];
                var responseData = response?.ToJsonWithSensitiveFilter(new string[] { "Data" });
                var requestPath = httpContext.Request.Path;
                var requestParams = request.ToJsonWithSensitiveFilter(new string[] { "Password" });
                var requestName = typeof(TRequest).Name;
                var requestMethod = httpContext.Request.Method;
                var userName = _currentUserService.UserName ?? "匿名访问";
                var clientIP = httpContext.Connection.RemoteIpAddress?.ToString();
                var statusCode = httpContext.Response.StatusCode;
                var loggerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var elapsedMilliseconds = _timer.ElapsedMilliseconds;

                var message = "";
                var succeeded = true;

                if (userName == "匿名访问" && request is LoginByUserNameAndPasswordCommand command)
                {
                    userName = command.UserName;
                }

                if (response is Result result)
                {
                    message = result.Message;
                    succeeded = result.Succeeded;
                }

                if (isException)
                {
                    _logger.LogCritical(MessageTemplate.ActivityHistoryLog,
                                        id, loggerName, userAgent, responseData, requestParams, requestPath, requestName, requestMethod, userName, clientIP, statusCode, message, loggerTime, elapsedMilliseconds);
                }
                if (!succeeded && !isException)
                {
                    _logger.LogError(MessageTemplate.ActivityHistoryLog,
                                        id, loggerName, userAgent, responseData, requestParams, requestPath, requestName, requestMethod, userName, clientIP, statusCode, message, loggerTime, elapsedMilliseconds);
                }
                if (succeeded && elapsedMilliseconds < 500)
                {
                    _logger.LogInformation(MessageTemplate.ActivityHistoryLog,
                                       id, loggerName, userAgent, responseData, requestParams, requestPath, requestName, requestMethod, userName, clientIP, statusCode, message, loggerTime, elapsedMilliseconds);
                }
                if (succeeded && elapsedMilliseconds > 500)
                {
                    _logger.LogWarning(MessageTemplate.ActivityHistoryLog,
                                        id, loggerName, userAgent, responseData, requestParams, requestPath, requestName, requestMethod, userName, clientIP, statusCode, message, loggerTime, elapsedMilliseconds);
                }
            }
        }
    }
}

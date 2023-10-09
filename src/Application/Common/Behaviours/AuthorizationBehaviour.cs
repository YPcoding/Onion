using Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Application.Common.Behaviours;

/// <summary>
/// 检查授权行为
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MenuDomainService _menuService;
    private readonly IOptions<SystemSettings> _optSystemSettings;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SystemSettings> optSystemSettings,
        MenuDomainService menuService)
    {
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _optSystemSettings = optSystemSettings;
        _menuService = menuService;
    }

    /// <summary>
    /// 检测授权逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="next">进入下一管道</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>进入下一管道</returns>
    /// <exception cref="ForbiddenAccessException">未授权时的异常处理</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isInPermissionWhiteList = _optSystemSettings.Value.PermissionWhiteListUserNames.Contains(_currentUserService.UserName!);
        if (isInPermissionWhiteList)
        {
            return await next().ConfigureAwait(false);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var controllerName = routeData?.Values["controller"]?.ToString();
        var actionName = routeData?.Values["action"]?.ToString();
        var controllerType = GetControllerType(controllerName!);

        if (controllerType != null)
        {
            var actionInfo = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == actionName);

            if (actionInfo != null && actionInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return await next().ConfigureAwait(false);
            }
        }

        //判断是否授权
        if (await IsAuthorizedAsync(httpContext!))
        {
            return await next().ConfigureAwait(false);
        }

        const string errorMessage = "您无权访问此资源";
        throw new ForbiddenAccessException(errorMessage);
    }

    /// <summary>
    /// 是否授权
    /// </summary>
    /// <param name="permissions">用户权限</param>
    /// <param name="apiPath">接口路径</param>
    /// <returns>返回授权结果</returns>
    private bool IsAuthorized(IEnumerable<string>? permissions, string apiPath)
    {
        return permissions?.Any(x => apiPath.ToLower().StartsWith(x.ToLower())) ?? false;
    }

    private async Task<bool> IsAuthorizedAsync(HttpContext httpContext)
    {
        var apiPath = httpContext?.Request?.Path.Value ?? "";
        var userPermissions = (await _menuService.GetPermissionsAsync(_currentUserService.CurrentUserId))
            .Select(s=>s.Url);

        return IsAuthorized(userPermissions!, apiPath);
    }

    /// <summary>
    /// 获取控制器类型
    /// </summary>
    /// <param name="controllerName">控制器名称</param>
    /// <returns>返回控制器类型</returns>
    private Type GetControllerType(string controllerName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => typeof(ControllerBase).IsAssignableFrom(t) 
            && string.Equals(t.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))!;
    }
}

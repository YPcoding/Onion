using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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
    private readonly PermissionDomainService _permissionService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        PermissionDomainService permissionService)
    {
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
    }

    /// <summary>
    /// 检测授权逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="next">进入下一管道</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>进入下一管道</returns>
    /// <exception cref="ForbiddenAccessException">未授权时的异常处理</exception>
    public async Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var controllerName = routeData?.Values["controller"]?.ToString();
        var actionName = routeData?.Values["action"]?.ToString();
        var controllerType = GetControllerType(controllerName!);

        if (controllerType != null)
        {
            var actionInfo = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == actionName);

            //检测是否可以匿名访问
            if (actionInfo != null && actionInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return await next().ConfigureAwait(false);
            }
        }

        //获取请求的接口路径
        var apiPath = httpContext?.Request?.Path.Value ?? "";
        //获取当前用户的权限
        var userPermissions = await _permissionService.GetPermissionsByUserIdAsync(_currentUserService.CurrentUserId);

        //检查用户是否拥有访问此路径的权限
        if (IsAuthorized(userPermissions, apiPath)) 
        {
            return await next().ConfigureAwait(false);
        }

        var errorMessage = "您无权访问此资源";
        throw new ForbiddenAccessException(errorMessage);
    }

    /// <summary>
    /// 是否授权
    /// </summary>
    /// <param name="permissions">用户权限</param>
    /// <param name="apiPath">接口路径</param>
    /// <returns>返回授权结果</returns>
    private bool IsAuthorized(List<Permission> permissions, string apiPath)
    {
        return permissions.Any(x => x.Path == apiPath);
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
            .FirstOrDefault(t => typeof(ControllerBase).IsAssignableFrom(t) && string.Equals(t.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))!;
    }
}

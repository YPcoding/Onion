using Domain.Entities;
using Domain.Enums;
using Domain.Services;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Application.Common.Behaviours;

/// <summary>
/// �����Ȩ��Ϊ
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
    /// �����Ȩ�߼�
    /// </summary>
    /// <param name="request">�������</param>
    /// <param name="next">������һ�ܵ�</param>
    /// <param name="cancellationToken">ȡ�����</param>
    /// <returns>������һ�ܵ�</returns>
    /// <exception cref="ForbiddenAccessException">δ��Ȩʱ���쳣����</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

            if (actionInfo != null && actionInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return await next().ConfigureAwait(false);
            }
        }

        //�ж��Ƿ���Ȩ
        if (await IsAuthorizedAsync(httpContext!))
        {
            return await next().ConfigureAwait(false);
        }

        const string errorMessage = "����Ȩ���ʴ���Դ";
        throw new ForbiddenAccessException(errorMessage);
    }

    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="permissions">�û�Ȩ��</param>
    /// <param name="apiPath">�ӿ�·��</param>
    /// <returns>������Ȩ���</returns>
    private bool IsAuthorized(IEnumerable<Permission> permissions, string apiPath)
    {
        return permissions.Any(x =>
            x.Type == PermissionType.Dot &&           // Ȩ������Ϊ Dot
            !string.IsNullOrEmpty(x.Path) &&           // Path ��Ϊ��
            x.Path != "/" &&                           // Path ��Ϊ��·��
            apiPath.StartsWith(x.Path) &&                // apiPath �� x.Path ��ͷ
            !string.IsNullOrEmpty(x.HttpMethods)       // HttpMethods ��Ϊ��
        );
    }

    private async Task<bool> IsAuthorizedAsync(HttpContext httpContext)
    {
        var apiPath = httpContext?.Request?.Path.Value ?? "";
        var userPermissions = await _permissionService.GetPermissionsByUserIdAsync(_currentUserService.CurrentUserId);
        return IsAuthorized(userPermissions, apiPath);
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    /// <param name="controllerName">����������</param>
    /// <returns>���ؿ���������</returns>
    private Type GetControllerType(string controllerName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => typeof(ControllerBase).IsAssignableFrom(t) 
            && string.Equals(t.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))!;
    }
}

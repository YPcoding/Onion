using Domain.Entities;
using Domain.Services;
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

            //����Ƿ������������
            if (actionInfo != null && actionInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return await next().ConfigureAwait(false);
            }
        }

        //��ȡ����Ľӿ�·��
        var apiPath = httpContext?.Request?.Path.Value ?? "";
        //��ȡ��ǰ�û���Ȩ��
        var userPermissions = await _permissionService.GetPermissionsByUserIdAsync(_currentUserService.CurrentUserId);

        //����û��Ƿ�ӵ�з��ʴ�·����Ȩ��
        if (IsAuthorized(userPermissions, apiPath)) 
        {
            return await next().ConfigureAwait(false);
        }

        var errorMessage = "����Ȩ���ʴ���Դ";
        throw new ForbiddenAccessException(errorMessage);
    }

    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="permissions">�û�Ȩ��</param>
    /// <param name="apiPath">�ӿ�·��</param>
    /// <returns>������Ȩ���</returns>
    private bool IsAuthorized(List<Permission> permissions, string apiPath)
    {
        return permissions.Any(x => x.Path == apiPath);
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
            .FirstOrDefault(t => typeof(ControllerBase).IsAssignableFrom(t) && string.Equals(t.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))!;
    }
}

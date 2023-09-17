using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var actionDescriptor = httpContext?.Features.Get<ControllerActionDescriptor>();
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

        var apiPath = httpContext?.Request?.Path.Value ?? "";
        if (IsAuthorized(_currentUserService?.AssignRoles!, apiPath)) 
        {
            return await next().ConfigureAwait(false);
        }

        throw new ForbiddenAccessException("您无权访问此资源");
    }

    private bool IsAuthorized(string[] currentUserPpiPaths, string apiPath)
    {
        return currentUserPpiPaths?.Contains(apiPath) ?? false;
    }

    private Type GetControllerType(string controllerName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => typeof(ControllerBase).IsAssignableFrom(t) && string.Equals(t.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))!;
    }
}

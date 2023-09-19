using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Encodings.Web;

namespace Infrastructure.Auth;

/// <summary>
/// 响应认证处理器
/// </summary>
public class ResponseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ResponseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Response.WriteAsync(JsonConvert.SerializeObject(
            new 
            {
                code = StatusCodes.Status401Unauthorized,
                succeeded = false,
                errors = new string[] { "未授权" },
                errorMessage = "未授权"
            },
            new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = StatusCodes.Status403Forbidden;
        await Response.WriteAsync(JsonConvert.SerializeObject(
            new
            {
                code = StatusCodes.Status403Forbidden,
                succeeded = false,
                errors = new string[] { "禁止访问" },
                errorMessage = "禁止访问"
            },
            new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        ));
    }
}

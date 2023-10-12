using Application.Common.Configurations;
using CInfrastructure.Persistence;
using FluentValidation;
using Infrastructure.Auth;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();//
        services.AddScoped<IDbCommandInterceptor, ExecutionTimeInterceptor>();
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Key));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Key));
        services.Configure<SystemSettings>(configuration.GetSection(SystemSettings.Key));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        services.AddHostedService<QuartzStatusBackgroundService>();
        services.AddDbContext<ApplicationDbContext>((p, m) =>
        {
            var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
            m.AddInterceptors(p.GetServices<IDbCommandInterceptor>());
            m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            m.UseLoggerFactory(LoggerFactory.Create(builder => // 配置 SQL 查询日志
            {
                builder
                    .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                    .AddConsole();
            }));
        });
        services.AddScoped<IDbContextFactory<ApplicationDbContext>, ContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddServices();
        services.AddAuthorization();
        JwtSettings jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = nameof(ResponseAuthenticationHandler); //401
            options.DefaultForbidScheme = nameof(ResponseAuthenticationHandler);    //403
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/signalRHub")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        }).AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(nameof(ResponseAuthenticationHandler), o => { });
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.WriteIndented = true; // 格式化输出
            options.JsonSerializerOptions.AllowTrailingCommas = true; // 允许尾随逗号
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // 使用驼峰命名法
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // 忽略默认值属性
            options.JsonSerializerOptions.Converters.Add(new LongConverter());//解决long类型数据精度丢失问题
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // 枚举使用描述表示
            options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter("yyyy-MM-dd HH:mm:ss.fff")); // 自定义日期格式化
        });

        services.AddEndpointsApiExplorer();
        services.Configure<ApiBehaviorOptions>(options => //请求参数校验
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState
                   .Where(e => e.Value?.Errors.Count > 0)
                   .Select(e => e.Value?.Errors.First().ErrorMessage)
                   .ToList();

                throw new ValidationException(errors?.Join("|"));
            };
        });
        #region Swagger文档
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Onion API文档 V1", Version = "v1" });
            c.AddServer(new OpenApiServer()
            {
                Url = "",
                Description = "Onion API文档"
            });
            c.CustomOperationIds(apiDesc =>
            {
                var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                return controllerAction!.ControllerName + "-" + controllerAction.ActionName;
            });
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Application.xml"), true);
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Domain.xml"), true);
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Infrastructure.xml"), true);
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WebAPI.xml"), true);
        });
        #endregion

        #region 雪花ID
        SnowFlakeSettings snowFlakeSettings = configuration.GetSection(SnowFlakeSettings.Key).Get<SnowFlakeSettings>()!;
        services.AddSingleton<ISnowFlakeService>(new SnowFlakeService(snowFlakeSettings.WorkerId, snowFlakeSettings.DataCenterId));
        #endregion

        services.AddSignalR();

        //响应压缩
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true; // 启用 HTTPS 时也应用压缩
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/plain",
                "text/html",
                "text/css",
                "application/javascript",
                "text/javascript",
                "application/json",
                "text/json",
                "application/xml",
                "text/xml",
                "image/svg+xml"
            });
        });

        return services;
    }
}


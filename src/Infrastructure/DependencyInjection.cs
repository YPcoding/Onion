﻿using Microsoft.Extensions.Options;
using Application.Common.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Common.Helper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Masuit.Tools;
using FluentValidation;
using CInfrastructure.Persistence;
using Newtonsoft.Json;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Key));
        services.Configure<SystemSettings>(configuration.GetSection(SystemSettings.Key));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        services.AddDbContext<ApplicationDbContext>((p, m) =>
        {
            var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
            m.AddInterceptors(new ExecutionTimeInterceptor());
            m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            // 配置 SQL 查询日志
            m.UseLoggerFactory(LoggerFactory.Create(builder =>
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
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey))
            };
        }).AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(nameof(ResponseAuthenticationHandler), o => { }); ;
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            //格式化时间格式
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            //解决long类型数据精度丢失问题
            options.SerializerSettings.ContractResolver = new CustomContractResolver();
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; // 忽略空值属性
            options.SerializerSettings.NullValueHandling = (NullValueHandling)DefaultValueHandling.Ignore;// 忽略默认值属性
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
        return services;
    }
}


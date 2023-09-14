using Microsoft.Extensions.Options;
using Application.Common.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Common.Helper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        services.AddDbContext<ApplicationDbContext>((p, m) =>
        {
            var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
            m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
        });
        services.AddScoped<IDbContextFactory<ApplicationDbContext>, ContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        services.AddServices();
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            options.SerializerSettings.ContractResolver = new CustomContractResolver();
        });
        services.AddEndpointsApiExplorer();
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
                return controllerAction.ControllerName + "-" + controllerAction.ActionName;
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


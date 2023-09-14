using Infrastructure.Middlewares;
namespace Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ExceptionHandlingMiddleware>();
    }
}

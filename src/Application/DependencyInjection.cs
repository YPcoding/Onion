using Application.Common.Behaviours;
using Application.Common.Jobs;
using Application.Common.PublishStrategies;
using MediatR.Pipeline;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config => {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.NotificationPublisher = new ParallelNoWaitPublisher();
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>));
            config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });
        services.AddLazyCache();
        services.AddQuartz();
        //services.AddSingleton<IJobFactory, SingletonJobFactory>();
        //services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<IJobFactory>(provider => new SingletonJobFactory(provider));

        return services;
    }
}

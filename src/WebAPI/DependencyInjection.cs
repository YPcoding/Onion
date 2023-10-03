using Application.Common.Mappings;
using AutoMapper;


namespace WebAPI;

public static class DependencyInjection
{
    /// <summary>
    /// 要扫描的程序集名称
    /// 默认为[^Application.|^Domain.|^Infrastructure.|^WebAPI.]多个使用|分隔
    /// </summary>
    public static string MatchAssemblies = "^Common.|^Application.|^Domain.|^Infrastructure.|^WebAPI.";

    /// <summary>
    /// 自动依赖注入
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        #region 依赖注入       
        var baseType = typeof(IDependency);
        var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        var getFiles = Directory.GetFiles(path, "*.dll").Where(Match);
        var referencedAssemblies = getFiles.Select(Assembly.LoadFrom).ToList();    

        var ss = referencedAssemblies.SelectMany(o => o.GetTypes());

        var types = referencedAssemblies
            .SelectMany(a => a.DefinedTypes)
            .Select(type => type.AsType())
            .Where(x => baseType.IsAssignableFrom(x)
            && (x != baseType)
            && !(x == typeof(IScopedDependency)
            || x == typeof(ISingletonDependency)
            || x == typeof(ITransientDependency)))
            .ToList();
        var implementTypes = types.Where(x => x.IsClass).ToList();
        var interfaceTypes = types.Where(x => x.IsInterface).ToList();
        foreach (var implementType in implementTypes)
        {
            if (typeof(IScopedDependency).IsAssignableFrom(implementType))
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));               
                if (interfaceType != null)//继承接口的注入
                    services.AddScoped(interfaceType, implementType);
                else //无继承接口的注入
                    services.AddScoped(implementType);
            }
            else if (typeof(ISingletonDependency).IsAssignableFrom(implementType))
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null)//继承接口的注入
                    services.AddSingleton(interfaceType, implementType);
                else//无继承接口的注入
                    services.AddSingleton(implementType);
            }
            else
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));                
                if (interfaceType != null)//继承接口的注入
                    services.AddTransient(interfaceType, implementType);
                else//无继承接口的注入
                    services.AddTransient(implementType);
            }
        }
        #endregion
        return services;
    }

    /// <summary>
    /// 程序集是否匹配
    /// </summary>
    public static bool Match(string assemblyName)
    {
        assemblyName = Path.GetFileName(assemblyName);
        return Regex.IsMatch(assemblyName, MatchAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    /// <summary>
    /// 使用AutoMapper自动映射拥有MapAttribute的类
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">自定义配置</param>
    public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configure = null)
    {
        List<(Type from, Type[] targets)> maps = new List<(Type from, Type[] targets)>();
        Assembly[] allAssemblies = new Assembly[]
        {
              Assembly.Load("Application"),
              Assembly.Load("Domain"),
              Assembly.Load("Infrastructure"),
              Assembly.Load("WebAPI"),
        };
        Type[] allTypes = allAssemblies.SelectMany(x => x.GetTypes()).ToArray();

        maps.AddRange(allTypes.Where(x => x.GetCustomAttribute<MapAttribute>() != null)
            .Select(x => (x, x.GetCustomAttribute<MapAttribute>()!.TargetTypes)));

        var configuration = new MapperConfiguration(cfg =>
        {
            maps.ForEach(aMap =>
            {
                aMap.targets.ToList().ForEach(aTarget =>
                {
                    cfg.CreateMap(aMap.from, aTarget).IgnoreAllNonExisting(aMap.from, aTarget).ReverseMap();
                });
            });

            cfg.AddMaps(allAssemblies);

            //自定义映射
            configure?.Invoke(cfg);
        });

#if DEBUG
        //只在Debug时检查配置
        configuration.AssertConfigurationIsValid();
#endif
        services.AddSingleton(configuration.CreateMapper());

        return services;
    }

    /// <summary>
    /// 忽略所有不匹配的属性。
    /// </summary>
    /// <param name="expression">配置表达式</param>
    /// <param name="from">源类型</param>
    /// <param name="to">目标类型</param>
    /// <returns></returns>
    public static IMappingExpression IgnoreAllNonExisting(this IMappingExpression expression, Type from, Type to)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        to.GetProperties(flags).Where(x => from.GetProperty(x.Name, flags) == null).ForEach(aProperty =>
        {
            expression.ForMember(aProperty.Name, opt => opt.Ignore());
        });

        return expression;
    }

    /// <summary>
    /// 忽略所有不匹配的属性。
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="expression">配置表达式</param>
    /// <returns></returns>
    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
    {
        Type from = typeof(TSource);
        Type to = typeof(TDestination);
        var flags = BindingFlags.Public | BindingFlags.Instance;
        to.GetProperties(flags).Where(x => from.GetProperty(x.Name, flags) == null).ForEach(aProperty =>
        {
            expression.ForMember(aProperty.Name, opt => opt.Ignore());
        });

        return expression;
    }
}


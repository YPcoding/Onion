using Application.Common.Extensions;
using Domain.Entities.Job;
using Application.Features.ScheduledJobs.DTOs;
using Application.Features.ScheduledJobs.Specifications;
using System.Text.RegularExpressions;
using System.Reflection;
using Quartz;
using Application.Common.Jobs;
using Domain.Entities.Loggers;
using System.Security.AccessControl;

namespace Application.Features.ScheduledJobs.Queries.Pagination;

/// <summary>
/// 定时任务分页查询
/// </summary>
[Description("计划任务列表查询")]
public class ScheduledJobsWithPaginationQuery : ScheduledJobAdvancedFilter, IRequest<Result<PaginatedData<ScheduledJobDto>>>
{
    [JsonIgnore]
    public ScheduledJobAdvancedPaginationSpec Specification => new ScheduledJobAdvancedPaginationSpec(this);
}

/// <summary>
/// 计划任务日志分页查询
/// </summary>
[Description("计划任务日志分页查询")]
public class ScheduledJobLogsWithPaginationQuery : ScheduledJobLogAdvancedFilter, IRequest<Result<PaginatedData<ScheduledJobLogDto>>>
{
    [JsonIgnore]
    public ScheduledJobLogAdvancedPaginationSpec Specification => new ScheduledJobLogAdvancedPaginationSpec(this);
}

/// <summary>
/// 计划任务分组列表查询
/// </summary>
[Description("计划任务分组列表查询")]
public class ScheduledJobGroupQuery:IRequest<Result<IEnumerable<JobGroupDto>>>
{
}

/// <summary>
/// 处理程序
/// </summary>
public class ScheduledJobsWithPaginationQueryHandler :
    IRequestHandler<ScheduledJobsWithPaginationQuery, Result<PaginatedData<ScheduledJobDto>>>,
    IRequestHandler<ScheduledJobGroupQuery, Result<IEnumerable<JobGroupDto>>>,
    IRequestHandler<ScheduledJobLogsWithPaginationQuery, Result<PaginatedData<ScheduledJobLogDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ScheduledJobsWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回定时任务分页数据</returns>
    public async Task<Result<PaginatedData<ScheduledJobDto>>> Handle(
        ScheduledJobsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        request.PageSize = int.MaxValue;
        var scheduledjobs = await _context.ScheduledJobs
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<ScheduledJob, ScheduledJobDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<ScheduledJobDto>>.SuccessAsync(scheduledjobs);
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回定时任务分页数据</returns>
    public async Task<Result<IEnumerable<JobGroupDto>>> Handle(ScheduledJobGroupQuery request, CancellationToken cancellationToken)
    {
        var jobGroups = new List<JobGroupDto>();

        var baseQuartzType = typeof(IJob);
        var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        var getFiles = Directory.GetFiles(path, "Application.dll");
        var referencedAssemblies = getFiles.Select(Assembly.LoadFrom).ToList();

        var types = referencedAssemblies
           .SelectMany(a => a.DefinedTypes)
           .Select(type => type.AsType())
           .Where(x => baseQuartzType.IsAssignableFrom(x)
           && (x != baseQuartzType)
           && !(x == typeof(IJob)))
           .ToList();

        // 遍历每个文件
        foreach (var type in types)
        {
            var jobGroup = new JobGroupDto
            {
                Value = type?.FullName!
            };

            //获取参数，转Json字符串
            Type baseType = type?.BaseType!;
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(JobParameterAbstractBase<>))
            {
                Type parameterType = baseType?.GetGenericArguments()[0]!;
                if (parameterType != null)
                {
                    object parameterInstance = Activator.CreateInstance(parameterType)!;
                    string parameterJson = parameterInstance.ToReadableJson();
                    jobGroup.ParameterJson = parameterJson;

                    PropertyInfo[] properties = parameterType.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string propertyName = property.Name;
                        object? propertyValue = property.GetValue(parameterInstance);

                        var descriptionAttribute = (DescriptionAttribute)property?.GetCustomAttribute(typeof(DescriptionAttribute))!;
                        string? propertyDescription = descriptionAttribute?.Description;
                        jobGroup.Description += $"*{propertyName}:{propertyDescription}\r\n";
                    }
                }
            }

            jobGroup.Label = type.CustomAttributes
                .Where(x => x.AttributeType.Name == "DescriptionAttribute")
                ?.FirstOrDefault()?.ConstructorArguments?
                .FirstOrDefault().Value?.ToString() ?? Guid.NewGuid().ToString().Replace("-", "");

            jobGroups.Add(jobGroup);
        }

        return await Result<IEnumerable<JobGroupDto>>.SuccessAsync(jobGroups);
    }

    public async Task<Result<PaginatedData<ScheduledJobLogDto>>> Handle(ScheduledJobLogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var loggers = await _context.Loggers
         .OrderBy($"{request.OrderBy} {request.SortDirection}")
         .ProjectToPaginatedDataAsync<Logger, ScheduledJobLogDto>(
         request.Specification,
         request.PageNumber,
         request.PageSize,
         _mapper.ConfigurationProvider,
         cancellationToken);

        return await Result<PaginatedData<ScheduledJobLogDto>>.SuccessAsync(loggers);
    }
}

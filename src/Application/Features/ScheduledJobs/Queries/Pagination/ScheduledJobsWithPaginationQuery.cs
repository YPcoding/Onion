using Application.Common.Extensions;
using Domain.Entities.Job;
using Application.Features.ScheduledJobs.DTOs;
using Application.Features.ScheduledJobs.Specifications;
using System.Text.RegularExpressions;
using System.Reflection;
using Quartz;
using Application.Common.Jobs;

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
/// 定时任务分页查询
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
    IRequestHandler<ScheduledJobGroupQuery, Result<IEnumerable<JobGroupDto>>>
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
        string currentDirectory = Directory.GetCurrentDirectory();
        string parentDirectory = Path.GetDirectoryName(currentDirectory)!;

        string entitiesPath = $"{parentDirectory}\\Application\\Common\\Jobs";

        // 获取文件夹内的所有文件
        string[] files = Directory.GetFiles(entitiesPath, "*.cs", SearchOption.AllDirectories);


        // 遍历每个文件
        foreach (string filePath in files)
        {
            // 读取文件内容
            string fileContent = File.ReadAllText(filePath);
            string namespaceName = string.Empty;
            string className = string.Empty;

            // 使用正则表达式查找命名空间
            Match namespaceMatch = Regex.Match(fileContent, @"namespace\s+([\w.]+)");
            if (namespaceMatch.Success)
            {
                namespaceName = namespaceMatch.Groups[1].Value;
            }

            // 使用正则表达式查找当前类名称
            Match classMatch = Regex.Match(fileContent, @"class\s+([\w.]+)");
            if (classMatch.Success)
            {
                className = classMatch.Groups[1].Value;
            }

            var jobGroup = new JobGroupDto();
            jobGroup.Value = ($"{namespaceName}.{className}");
            Assembly assembly = Assembly.Load(jobGroup.Value.Split('.')[0].ToString());
            var type =  assembly.GetType(jobGroup.Value)!;

            if (!typeof(IJob).IsAssignableFrom(type)) continue;

            //获取参数，转Json字符串
            Type baseType = type.BaseType!;
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
}

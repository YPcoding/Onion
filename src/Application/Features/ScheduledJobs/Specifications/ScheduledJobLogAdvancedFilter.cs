namespace Application.Features.ScheduledJobs.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class ScheduledJobLogAdvancedFilter : PaginationFilter
{       
    public string? JobName { get; set; }

    public string? JobGroup { get; set; }
}
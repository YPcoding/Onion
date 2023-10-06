using Domain.Entities.Departments;
namespace Application.Features.Departments.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class DepartmentAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 部门名称
    /// </summary>
    [Description("部门名称")]
    public string? DepartmentName { get; set; }
       
    /// <summary>
    /// 描述
    /// </summary>
    [Description("描述")]
    public string? Description { get; set; }
}
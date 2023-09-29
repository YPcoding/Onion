using Domain.Entities;
namespace Application.Features.TestTables.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class TestTableAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 名称
    /// </summary>
    [Description("名称")]
    public string? Name { get; set; }
       
    /// <summary>
    /// 描述
    /// </summary>
    [Description("描述")]
    public string? Description { get; set; }
       
    /// <summary>
    /// 状态
    /// </summary>
    [Description("状态")]
    public bool? Stuts { get; set; }
}
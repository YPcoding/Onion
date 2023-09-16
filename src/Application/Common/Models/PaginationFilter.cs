using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models;

/// <summary>
/// 分页过滤器
/// </summary>
public partial class PaginationFilter : BaseFilter
{
    /// <summary>
    /// 页码
    /// </summary>
    [Required(ErrorMessage = "页码是必填的")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    [Required(ErrorMessage = "每页大小是必填的")]
    public int PageSize { get; set; } = 15;

    /// <summary>
    /// 排序字段；如：Id
    /// </summary>
    public string? OrderBy { get; set; } = "Id";

    /// <summary>
    /// 排序方向：1、Descending 2、Ascending
    /// </summary>
    public string? SortDirection { get; set; } = "Descending";

    public override string ToString() => $"PageNumber:{PageNumber},PageSize:{PageSize},OrderBy:{OrderBy},SortDirection:{SortDirection},Keyword:{Keyword}";
}

/// <summary>
/// 基础过滤器
/// </summary>
public class BaseFilter
{
    /// <summary>
    /// 模糊查询关键字
    /// </summary>
    public string? Keyword { get; set; }
}


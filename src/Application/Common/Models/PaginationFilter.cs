using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models;

/// <summary>
/// ��ҳ������
/// </summary>
public partial class PaginationFilter : BaseFilter
{
    /// <summary>
    /// ҳ��
    /// </summary>
    [Required(ErrorMessage = "ҳ���Ǳ����")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// ÿҳ��С
    /// </summary>
    [Required(ErrorMessage = "ÿҳ��С�Ǳ����")]
    public int PageSize { get; set; } = 15;

    /// <summary>
    /// �����ֶΣ��磺Id
    /// </summary>
    public string? OrderBy { get; set; } = "Id";

    /// <summary>
    /// ������1��Descending 2��Ascending
    /// </summary>
    public string? SortDirection { get; set; } = "Descending";

    public override string ToString() => $"PageNumber:{PageNumber},PageSize:{PageSize},OrderBy:{OrderBy},SortDirection:{SortDirection},Keyword:{Keyword}";
}

/// <summary>
/// ����������
/// </summary>
public class BaseFilter
{
    /// <summary>
    /// ģ����ѯ�ؼ���
    /// </summary>
    public string? Keyword { get; set; }
}


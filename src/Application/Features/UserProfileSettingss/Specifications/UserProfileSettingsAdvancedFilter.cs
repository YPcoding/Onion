using Domain.Entities.Settings;
namespace Application.Features.UserProfileSettings.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class UserProfileSettingsAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 设置名称
    /// </summary>
    [Description("设置名称")]
    public string? SettingName { get; set; }
       
    /// <summary>
    /// 设置相关值
    /// </summary>
    [Description("设置相关值")]
    public string? SettingValue { get; set; }
}
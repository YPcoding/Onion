using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
/// 菜单枚举
/// </summary>
[Description("菜单枚举")]
public enum MetaType
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Description("菜单")]
    Menu = 0,
    /// <summary>
    /// Iframe
    /// </summary>
    [Description("Iframe")]
    iframe = 1,
    /// <summary>
    /// 外链
    /// </summary>
    [Description("外链")]
    link = 2,
    /// <summary>
    /// 按钮
    /// </summary>
    [Description("按钮")]
    Button,
    /// <summary>
    /// API
    /// </summary>
    [Description("Api")]
    Api
}

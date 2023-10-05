using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
///性别
/// </summary>
[Description("性别")]
public enum GenderType
{
    /// <summary>
    /// 保密
    /// </summary>
    [Description("保密")]
    Secrecy = 0,
    /// <summary>
    /// 男
    /// </summary>
    [Description("男")]
    Male = 1,
    /// <summary>
    /// 女
    /// </summary>
    [Description("女")]
    Female = 2,
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Settings
{
    /// <summary>
    /// 个人设置
    /// </summary>
    [Description("个人设置")]
    public class UserProfileSetting : BaseAuditableEntity, IAuditTrial
    {
        public UserProfileSetting() { }
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        public User User { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("用户唯一标识")]
        public long UserId { get; set; }
        /// <summary>
        /// 设置名称
        /// </summary>
        [Description("设置名称")]
        public string SettingName { get; set; }
        /// <summary>
        /// 设置相关值
        /// </summary>
        [Description("设置相关值")]
        public string SettingValue { get; set; }
        /// <summary>
        /// 相关值类型
        /// </summary>
        [Description("相关值类型")]
        public string ValueType { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        [Description("默认值")]
        public string DefaultValue { get; set; }
    }
}

using Domain.Entities.Settings;

namespace Application.Features.UserProfileSettings.DTOs
{
    [Map(typeof(UserProfileSetting))]
    public class UserProfileSettingsDto
    {
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
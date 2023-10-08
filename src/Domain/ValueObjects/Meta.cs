using System.ComponentModel;

namespace Domain.ValueObjects
{
    /// <summary>
    /// 元信息
    /// </summary>
    public class Meta : ValueObject
    {
        static Meta()
        {
        }

        private Meta()
        {
        }

        public Meta(MetaType? type, string? title, string? icon, string? active, string? color, bool? fullpage, bool? hidden, bool? hiddenBreadcrumb, string? tag)
        {
            Title = title;
            Icon = icon;
            Active = active;
            Color = color;
            Type = type;
            Fullpage = fullpage;
            Hidden = hidden;
            HiddenBreadcrumb = hiddenBreadcrumb;
            Tag = tag;
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Description("显示名称")]
        public string? Title { get; private set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Description("图标")]
        public string? Icon { get; private set; }
        /// <summary>
        /// 菜单高亮
        /// </summary>
        [Description("菜单高亮")]
        public string? Active { get; private set; }
        /// <summary>
        /// 颜色
        /// </summary>
        [Description("颜色")]
        public string? Color { get; private set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public MetaType? Type { get; private set; }
        /// <summary>
        /// 整页路由
        /// </summary>
        [Description("整页路由")]
        public bool? Fullpage { get; private set; }
        /// <summary>
        /// 隐藏
        /// </summary>
        [Description("隐藏")]
        public bool? Hidden { get; private set; }
        /// <summary>
        /// 隐藏面包屑
        /// </summary>
        [Description("隐藏面包屑")]
        public bool? HiddenBreadcrumb { get; private set; }
        /// <summary>
        /// 标签
        /// </summary>
        [Description("标签")]
        public string? Tag { get; private set; }

        public static implicit operator string(Meta  meta)
        {
            return meta.ToString()!;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Title!;
            yield return Icon!;
            yield return Active!;
            yield return Color!;
            yield return Type!;
            yield return Fullpage!;
            yield return Hidden!;
            yield return HiddenBreadcrumb!;
            yield return Tag!;
        }
    }
}

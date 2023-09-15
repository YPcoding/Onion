using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Features.Users.DTOs
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Map(typeof(User))]
    public class UserDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户组件
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 标准化用户名
        /// </summary>
        public string? NormalizedUserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 标准化邮箱
        /// </summary>
        public string? NormalizedEmail { get; set; }

        /// <summary>
        /// 确认邮箱
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// 安全印章（修改敏感信息时必须修改）
        /// </summary>
        public string? SecurityStamp { get; set; }

        /// <summary>
        /// 并发标记
        /// </summary>
        public string? ConcurrencyStamp { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 确认手机号码
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 启用双因子
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 锁定结束时间
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 已锁定
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 访问失败次数
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 头像图片
        /// </summary>
        [Column(TypeName = "text")]
        public string? ProfilePictureDataUrl { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// 是否活跃
        /// </summary>
        public bool IsLive { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Created { get; set; }
    }
}

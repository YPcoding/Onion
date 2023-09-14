using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity
{
    /// <summary>
    ///用户表
    /// </summary>
    public class User : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string? UserName { get; set; }

        /// <summary>
        /// 标准化用户名
        /// </summary>
        public virtual string? NormalizedUserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string? Email { get; set; }

        /// <summary>
        /// 标准化邮箱
        /// </summary>
        public virtual string? NormalizedEmail { get; set; }

        /// <summary>
        /// 确认邮箱
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// 哈希密码
        /// </summary>
        public virtual string? PasswordHash { get; set; }

        /// <summary>
        /// 安全印章（修改敏感信息时必须修改）
        /// </summary>
        public virtual string? SecurityStamp { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string? PhoneNumber { get; set; }

        /// <summary>
        /// 确认手机号码
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 启用双因子
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 锁定结束时间
        /// </summary>
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 已锁定
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// 访问失败次数
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

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
    }
}

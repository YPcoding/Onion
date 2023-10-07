using Application.Features.Roles.DTOs;
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
        /// 唯一标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public long UserId 
        { 
            get 
            { 
                return Id; 
            } 
        }

        /// <summary>
        /// 上级唯一标识
        /// </summary>
        public long? SuperiorId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<RoleDto>? Roles { get; set; }

        /// <summary>
        /// 角色唯一标识
        /// </summary>
        public List<long>? RoleIds 
        {
            get 
            {
                return Roles?.Select(s => s.RoleId)?.ToList();
            }
        }

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

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Description("真实姓名")]
        public virtual string? Realname { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Description("昵称")]
        public virtual string? Nickname { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public virtual GenderType GenderType { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Description("生日")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [Description("个性签名")]
        public string? Signature { get; set; }

        /// <summary>
        /// 部门唯一标识
        /// </summary>
        [Description("部门唯一标识")]
        public long? DepartmentId { get; set; }
    }
}

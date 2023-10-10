using Domain.Entities.Departments;
using Domain.Entities.Loggers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity
{
    /// <summary>
    ///用户表
    /// </summary>
    [Description("用户管理")]
    public class User : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
            Loggers = new HashSet<Logger>();
        }


        /// <summary>
        /// 账号
        /// </summary>
        [Description("账号")]
        public virtual string? UserName { get; set; }

        /// <summary>
        /// 标准化用户名
        /// </summary>
        public virtual string? NormalizedUserName
        {
            get
            {
                return UserName?.ToUpper();
            }
            set { }
        }

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
        /// 邮箱
        /// </summary>
        public virtual string? Email { get; set; }

        /// <summary>
        /// 标准化邮箱
        /// </summary>
        public virtual string? NormalizedEmail 
        { 
            get 
            {
                return Email?.ToUpper();
            }
            set { }
        }

        /// <summary>
        /// 确认邮箱
        /// </summary>
        public virtual bool EmailConfirmed { get; set; } = false;

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
        public virtual bool PhoneNumberConfirmed { get; set; } = false;

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
        /// 是否在线
        /// </summary>
        public bool IsLive { get; set; } = false;

        /// <summary>
        /// 是否为系统账号
        /// </summary>
        public bool? IsSystemAccount { get; set; }

        /// <summary>
        /// 上级唯一标识
        /// </summary>
        public long? SuperiorId { get; set; } = null;
        [ForeignKey("SuperiorId")]
        public User? Superior { get; set; } = null;

        /// <summary>
        /// 部门
        /// </summary>
        public virtual long? DepartmentId { get; set; } = null;

        public virtual Department? Department { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 用户日志
        /// </summary>
        public ICollection<Logger> Loggers { get; set; }

        /// <summary>
        /// 创建密码
        /// </summary>
        /// <param name="password">密码</param>
        public string CreatePassword(string password) 
        {
            return password.MDString("Onion");
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="password"></param>
        public void ChangePassword(string password) 
        {
            PasswordHash = CreatePassword(password);
        }

        /// <summary>
        /// 密码比较
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CompareWithOldPassword (string password) 
        {
            return PasswordHash == CreatePassword(password);
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public void IsActived() 
        {
            IsActive = !IsActive;
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public void IsLived() 
        {
            IsLive = !IsLive;
        }

        /// <summary>
        /// 登录失败超出次数锁定
        /// </summary>
        /// <param name="failedLimitCount">登录失败限制次数</param>
        /// <param name="isLockoutEnabled">超出限制次数是否锁定</param>
        /// <param name="lockDurationMinutes">锁定时长</param>
        public void LoginFailedIfExceedConntWillBeLock(int failedLimitCount = 3, bool isLockoutEnabled = true, int lockDurationMinutes = 10)
        {
            AccessFailedCount += 1;
            if (AccessFailedCount % failedLimitCount == 0)
            {
                LockoutEnabled = isLockoutEnabled;
                LockoutEnd = DateTime.Now.AddMinutes(lockDurationMinutes);
            }
        }

        /// <summary>
        /// 锁定或解锁
        /// </summary>
        /// <param name="lockDurationMinutes">锁定时间</param>
        public void IsUnLock(int lockDurationMinutes = 10)
        {
            if (!LockoutEnabled) 
            {
                LockoutEnabled = true;
                LockoutEnd = DateTime.Now.AddMinutes(lockDurationMinutes);
            }
            else 
            {
                LockoutEnabled = false;
                LockoutEnd = DateTime.Now;
            }
        }
    }
}

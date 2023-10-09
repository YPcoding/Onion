using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Role : BaseAuditableEntity, IAuditTrial
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            RoleMenus = new HashSet<RoleMenu>();
        }
        public Role(string roleName,string description)
        {
            RoleName = roleName;
            Description = description;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; } 

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RoleMenu> RoleMenus { get;}
    }
}

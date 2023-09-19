namespace Domain.Entities
{
    public class Role : BaseAuditableEntity
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            RolePermissions = new HashSet<RolePermission>();
        }
        public Role(string roleName,string description)
        {
            RoleName = roleName;
            Description = description;
        }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}

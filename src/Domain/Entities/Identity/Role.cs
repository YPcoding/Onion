namespace Domain.Entities
{
    public class Role : BaseAuditableEntity
    {
        public Role()
        {

        }
        public Role(string roleName,string description)
        {
            RoleName = roleName;
            Description = description;
        }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

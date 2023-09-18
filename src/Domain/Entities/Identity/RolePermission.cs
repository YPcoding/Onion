namespace Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Permission Permission { get; set; }
        public long PermissionId { get; set; }
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}

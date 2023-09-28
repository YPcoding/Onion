using Domain.Entities;
using Domain.Entities.Audit;
using Domain.Entities.GenerateCode;
using Domain.Entities.Logger;

namespace Application.Common.Interfaces;

public  interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Logger> Loggers { get; set; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<TestTable> TestTables { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

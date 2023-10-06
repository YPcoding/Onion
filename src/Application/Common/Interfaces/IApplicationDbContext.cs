using Domain.Entities.Audit;
using Domain.Entities.Departments;
using Domain.Entities.Logger;
using Domain.Entities.Notifications;
using Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
    DbSet<Notification> Notifications { get; }
    DbSet<UserProfileSetting> UserProfileSettings { get; }
    DbSet<Department> Departments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DatabaseFacade Database { get; }
}

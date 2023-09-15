using Domain.Entities.Audit;
using Domain.Entities.Logger;

namespace Application.Common.Interfaces;

public  interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Logger> Loggers { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

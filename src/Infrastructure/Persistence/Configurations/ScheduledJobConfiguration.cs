using Domain.Entities.Identity;
using Domain.Entities.Job;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScheduledJobConfiguration : IEntityTypeConfiguration<ScheduledJob>
{
    public void Configure(EntityTypeBuilder<ScheduledJob> builder)
    {
        builder
            .Property(x => x.LastExecutionStatus).HasConversion<string>();
        builder
            .Property(x => x.Status).HasConversion<string>();
    }
}

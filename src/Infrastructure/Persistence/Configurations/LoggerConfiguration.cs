using Domain.Entities.Loggers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LoggerConfiguration : IEntityTypeConfiguration<Logger>
{
    public void Configure(EntityTypeBuilder<Logger> builder)
    {
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Timestamp)
            .HasConversion(
            v => v.ToString(),
            v => DateTime.Parse(v)
        );

        builder.Property(e => e.TS)
            .HasColumnName("_ts"); 
    }
}

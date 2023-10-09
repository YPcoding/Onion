using Domain.Entities.Logger;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LoggerConfiguration : IEntityTypeConfiguration<Logger>
{
    public void Configure(EntityTypeBuilder<Logger> builder)
    {
        builder.Property(e => e.Id)
            .HasColumnName("id"); 

        builder.Property(e => e.TS)
            .HasColumnName("_ts"); 
    }
}

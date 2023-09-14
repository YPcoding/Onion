using Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.HasOne(x => x.Owner)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.SetNull);
        builder.Navigation(e => e.Owner).AutoInclude();
        builder.Property(t => t.AuditType)
           .HasConversion<string>();
        builder.Property(e => e.AffectedColumns)!.HasStringListConversion();
        builder.Property(u => u.OldValues).HasJsonConversion();
        builder.Property(u => u.NewValues).HasJsonConversion();
        builder.Property(u => u.PrimaryKey).HasJsonConversion();
        builder.Ignore(x => x.TemporaryProperties);
        builder.Ignore(x => x.HasTemporaryProperties);
    }
}

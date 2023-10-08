using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder
           .OwnsOne(o => o.Meta, a =>
           {
               a.Property(t => t.Type)
               .HasConversion<string>();
               a.WithOwner();
           });
    }
}

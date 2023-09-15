using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Ignore(e => e.DomainEvents);
        builder.Property(t => t.UserName).HasMaxLength(20).IsRequired();
        builder.Property(t => t.NormalizedUserName).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(50);
        builder.Property(t => t.NormalizedEmail).HasMaxLength(50);
        builder.Property(t => t.EmailConfirmed).HasMaxLength(1);
        builder.Property(t => t.PasswordHash).HasMaxLength(100);
        builder.Property(t => t.SecurityStamp).HasMaxLength(36);
        builder.Property(t => t.PhoneNumber).HasMaxLength(20);
        builder.Property(t => t.PhoneNumberConfirmed).HasMaxLength(1);
        builder.Property(t => t.TwoFactorEnabled).HasMaxLength(1);
        builder.Property(t => t.LockoutEnabled).HasMaxLength(1); 
    }
}

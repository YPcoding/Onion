using Domain.Entities.Identity;
using Domain.Enums;
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
               .HasConversion(
                v => v.ToString().ToLower(),  // 将枚举值转换为小写字符串
                v => (MetaType)Enum.Parse(typeof(MetaType), v,true));
               a.WithOwner();
           });
    }
}

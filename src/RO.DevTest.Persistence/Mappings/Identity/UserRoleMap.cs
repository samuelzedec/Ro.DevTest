using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class UserRoleMap : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("aspnet_user_role");
            
        builder
            .Property(ur => ur.UserId)
            .HasColumnType("UUID")
            .HasColumnName("user_id")
            .IsRequired();
        
        builder
            .Property(ur => ur.RoleId)
            .HasColumnType("UUID")
            .HasColumnName("role_id")
            .IsRequired();
    }
}

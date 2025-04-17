using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class UserLoginMap : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("aspnet_user_login");
        
        builder
            .HasKey(ul => new { ul.UserId, ul.LoginProvider, ul.ProviderKey })
            .HasName("pk_aspnet_user_login_user_id_login_provider_provider_key");
        
        builder
            .Property(ul => ul.UserId)
            .HasColumnType("UUID")
            .HasColumnName("user_id")
            .IsRequired();
        
        builder
            .Property(ul => ul.LoginProvider)
            .HasColumnType("VARCHAR")
            .HasColumnName("login_provider")
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(ul => ul.ProviderKey)
            .HasColumnType("VARCHAR")
            .HasColumnName("provider_key")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(ul => ul.ProviderDisplayName)
            .HasColumnType("VARCHAR")
            .HasColumnName("provider_display_name")
            .HasMaxLength(255)
            .IsRequired();
    }
}
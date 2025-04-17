using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class UserTokenMap : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("aspnet_user_token");

        builder
            .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name })
            .HasName("pk_aspnet_user_token_user_id_login_provider_name");
        
        builder
            .Property(ut => ut.UserId)
            .HasColumnType("UUID")
            .HasColumnName("user_id")
            .IsRequired();
        
        builder
            .Property(ut => ut.LoginProvider)
            .HasColumnType("VARCHAR")
            .HasColumnName("login_provider")
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(ut => ut.Name)
            .HasColumnType("VARCHAR")
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(ut => ut.Value)
            .HasColumnType("VARCHAR")
            .HasColumnName("value")
            .HasMaxLength(755)
            .IsRequired();

        builder
            .Property(ut => ut.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired();
    }
}

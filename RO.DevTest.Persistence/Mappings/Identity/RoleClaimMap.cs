using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class RoleClaimMap : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("aspnet_role_claim");

        builder
            .HasKey(rc => rc.Id)
            .HasName("pk_aspnet_role_claim_id");

        builder
            .Property(rc => rc.Id)
            .HasColumnName("id")
            .UseIdentityColumn();
        
        builder
            .Property(rc => rc.RoleId)
            .HasColumnType("UUID") 
            .HasColumnName("role_id")
            .IsRequired();

        builder
            .Property(rc => rc.ClaimType)
            .HasColumnType("TEXT")
            .HasColumnName("claim_type")
            .HasMaxLength(80)
            .IsRequired();
        
        builder
            .Property(rc => rc.ClaimValue)
            .HasColumnType("TEXT")
            .HasColumnName("claim_value")
            .HasMaxLength(80)
            .IsRequired();
    }
}
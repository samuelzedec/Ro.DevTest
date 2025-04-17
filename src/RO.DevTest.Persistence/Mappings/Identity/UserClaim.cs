using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class IdentityUserClaimMap : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("aspnet_user_claim");

        builder
            .Property(uc => uc.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder
            .Property(uc => uc.UserId)
            .HasColumnType("UUID") 
            .HasColumnName("user_id")
            .IsRequired();

        builder
            .Property(uc => uc.ClaimType)
            .HasColumnType("TEXT")
            .HasColumnName("claim_type")
            .HasMaxLength(80)
            .IsRequired();
        
        builder
            .Property(uc => uc.ClaimValue)
            .HasColumnType("TEXT")
            .HasColumnName("claim_value")
            .HasMaxLength(80)
            .IsRequired();
    }
}

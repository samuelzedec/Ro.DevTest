using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class RoleMap : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("aspnet_role");

        builder
            .Property(r => r.Id)
            .HasColumnType("UUID")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(r => r.Name)
            .HasColumnType("VARCHAR")
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(r => r.NormalizedName)
            .HasColumnType("VARCHAR")
            .HasColumnName("normalized_name")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(r => r.NormalizedName)
            .HasDatabaseName("ix_aspnet_role_normalized_name")
            .IsUnique();
        
        builder
            .Property(u => u.ConcurrencyStamp)
            .HasColumnType("VARCHAR")
            .HasColumnName("concurrency_stamp")
            .HasMaxLength(36)
            .IsConcurrencyToken();
    }
}
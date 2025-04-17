using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Mappings.Identity;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("aspnet_user");
        
        builder
            .Property(u => u.Id)
            .HasColumnType("UUID")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(u => u.Name)
            .HasColumnType("VARCHAR")
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(u => u.UserName)
            .HasColumnType("VARCHAR")
            .HasColumnName("user_name")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(u => u.NormalizedUserName)
            .HasColumnType("VARCHAR")
            .HasColumnName("normalized_user_name")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("ix_aspnet_user_normalized_user_name")
            .IsUnique();

        builder
            .Property(u => u.Email)
            .HasColumnType("VARCHAR")
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(u => u.Email)
            .HasDatabaseName("ix_asp_net_user_email")
            .IsUnique();

        builder
            .Property(u => u.NormalizedEmail)
            .HasColumnType("VARCHAR")
            .HasColumnName("normalized_email")
            .HasMaxLength(255);

        builder
            .Property(u => u.EmailConfirmed)
            .HasColumnType("BOOLEAN")
            .HasColumnName("email_confirmed")
            .IsRequired();

        builder
            .Property(u => u.PasswordHash)
            .HasColumnType("VARCHAR")
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(u => u.PhoneNumber)
            .HasColumnType("VARCHAR")
            .HasColumnName("phone_number")
            .HasMaxLength(15)
            .IsRequired();

        builder
            .Property(u => u.PhoneNumberConfirmed)
            .HasColumnType("BOOLEAN")
            .HasColumnName("phone_number_confirmed")
            .IsRequired();
        
        builder
            .Property(u => u.ConcurrencyStamp)
            .HasColumnType("VARCHAR")
            .HasColumnName("concurrency_stamp")
            .HasMaxLength(36)
            .IsConcurrencyToken();

        builder
            .Property(u => u.SecurityStamp)
            .HasColumnType("VARCHAR")
            .HasColumnName("security_stamp")
            .HasMaxLength(36);

        builder
            .Property(u => u.LockoutEnd)
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasColumnName("lockout_end");

        builder
            .Property(u => u.LockoutEnabled)
            .HasColumnType("BOOLEAN")
            .HasColumnName("lockout_enabled")
            .IsRequired();

        builder
            .Property(u => u.AccessFailedCount)
            .HasColumnType("INTEGER")
            .HasColumnName("access_failed_count")
            .IsRequired();

        builder
            .Property(u => u.TwoFactorEnabled)
            .HasColumnType("BOOLEAN")
            .HasColumnName("two_factor_enabled")
            .IsRequired();
    }
}

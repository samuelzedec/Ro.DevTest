using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Mappings;

public class ProductMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("product");

        builder
            .HasKey(p => p.Id)
            .HasName("pk_product_id");

        builder
            .Property(p => p.Id)
            .HasColumnType("UUID")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder
            .Property(p => p.Name)
            .HasColumnType("VARCHAR(255)")
            .HasColumnName("name")
            .IsRequired();
        
        builder
            .Property(p => p.Description)
            .HasColumnType("VARCHAR(455)")
            .HasColumnName("description")
            .IsRequired();

        builder
            .Property(p => p.UnitPrice)
            .HasColumnType("NUMERIC(18,2)")
            .HasColumnName("unit_price")
            .IsRequired();

        builder
            .Property(p => p.AvailableQuantity)
            .HasColumnType("INTEGER")
            .HasColumnName("available_quantity")
            .IsRequired();

        builder
            .Property(p => p.ProductCategory)
            .HasColumnType("SMALLINT")
            .HasColumnName("product_category")
            .IsRequired();

        builder
            .HasOne(p => p.Admin)
            .WithMany()
            .HasForeignKey(s => s.AdminId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_product_admin")
            .IsRequired();

        builder
            .Property(p => p.CreatedOn)
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .HasColumnName("created_on")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(p => p.ModifiedOn)
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .HasColumnName("modified_on")
            .IsRequired(false);
        
        builder
            .Property(p => p.DeletedAt)
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .HasColumnName("deleted_at")
            .IsRequired(false);
    }
}
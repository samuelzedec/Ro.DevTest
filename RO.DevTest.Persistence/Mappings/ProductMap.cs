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
            .Property(p => p.Price)
            .HasColumnType("NUMERIC(18,2)")
            .HasColumnName("price")
            .IsRequired();

        builder
            .Property(p => p.Quantity)
            .HasColumnType("INTEGER")
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValueSql("0");

        builder
            .HasOne(p => p.Saller)
            .WithMany()
            .HasForeignKey(s => s.SallerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_sale_saller")
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
    }
}
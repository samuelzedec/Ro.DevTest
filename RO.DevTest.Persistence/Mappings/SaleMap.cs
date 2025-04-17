using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Mappings;

public class SaleMap : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sale");

        builder
            .HasKey(s => s.Id)
            .HasName("pk_sale_id");

        builder
            .Property(s => s.Id)
            .HasColumnType("UUID")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        // Saller
        builder
            .Property(s => s.SallerId)
            .HasColumnType("UUID")
            .HasColumnName("saller_id")
            .IsRequired();

        builder
            .HasOne(s => s.Saller)
            .WithMany()
            .HasForeignKey(s => s.SallerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_sales_saller");

        // Product
        builder
            .Property(s => s.ProductId)
            .HasColumnType("UUID")
            .HasColumnName("product_id")
            .IsRequired();

        builder
            .HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_sales_product");

        // Buyer
        builder
            .Property(s => s.BuyerId)
            .HasColumnType("UUID")
            .HasColumnName("buyer_id")
            .IsRequired();

        builder
            .HasOne(s => s.Buyer)
            .WithMany()
            .HasForeignKey(s => s.BuyerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_sales_buyer");

        // Others
        builder
            .Property(s => s.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("INTEGER")
            .IsRequired();

        builder
            .Property(s => s.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("NUMERIC(18,2)")
            .IsRequired();
        
        builder
            .Property(s => s.CreatedOn)
            .HasColumnName("created_on")
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(s => s.ModifiedOn)
            .HasColumnName("modified_on")
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .IsRequired(false);
    }
}
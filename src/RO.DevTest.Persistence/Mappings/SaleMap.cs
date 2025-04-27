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

        // Admin
        builder
            .Property(s => s.AdminId)
            .HasColumnType("UUID")
            .HasColumnName("admin_id")
            .IsRequired();

        builder
            .HasOne(s => s.Admin)
            .WithMany()
            .HasForeignKey(s => s.AdminId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_sales_admin");

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

        // Customer
        builder
            .Property(s => s.CustomerId)
            .HasColumnType("UUID")
            .HasColumnName("customer_id")
            .IsRequired();

        builder
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_sales_customer");

        // Others

        builder
            .Property(s => s.TransactionDate)
            .HasColumnName("transaction_date")
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .IsRequired();

        builder
            .Property(s => s.EPaymentMethod)
            .HasColumnName("payment_method")
            .HasColumnType("SMALLINT")
            .IsRequired();
        
        builder
            .Property(s => s.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("INTEGER")
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
        
        builder
            .Property(s => s.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("TIMESTAMP WITHOUT TIME ZONE")
            .IsRequired(false);
    }
}
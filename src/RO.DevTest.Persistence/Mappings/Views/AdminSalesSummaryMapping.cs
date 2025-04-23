using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.ReadModels;

namespace RO.DevTest.Persistence.Mappings.Views;

public class AdminSalesSummaryMapping : IEntityTypeConfiguration<AdminSalesSummary>
{
    public void Configure(EntityTypeBuilder<AdminSalesSummary> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_admin_product_sales");

        builder
            .Property(x => x.AdminId)
            .HasColumnName("admin_id");

        builder
            .Property(x => x.AdminUsername)
            .HasColumnName("admin_username");

        builder
            .Property(x => x.ProductId)
            .HasColumnName("product_id");

        builder
            .Property(x => x.ProductName)
            .HasColumnName("product_name");

        builder
            .Property(x => x.Year)
            .HasColumnName("year");

        builder
            .Property(x => x.Month)
            .HasColumnName("month");

        builder
            .Property(x => x.TotalValue)
            .HasColumnName("total_value");

        builder
            .Property(x => x.TransactionCount)
            .HasColumnName("transaction_count");
        
        builder
            .Property(x => x.TotalItemsSold)
            .HasColumnName("total_items_sold");
    }
}
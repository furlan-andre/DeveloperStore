using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(sale => sale.Id);
        builder.Property(sale => sale.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(sale => sale.SaleNumber)
            .HasColumnName("sale_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sale => sale.SaleDate)
            .HasColumnName("sale_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(sale => sale.TotalSaleAmount)
            .HasColumnName("total_sale_amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(sale => sale.Active)
            .HasColumnName("active")
            .IsRequired();

        builder.OwnsOne(sale => sale.Customer, customer =>
        {
            customer.Property(c => c.Id)
                .HasColumnName("customer_id")
                .HasColumnType("uuid")
                .IsRequired();

            customer.Property(c => c.Name)
                .HasColumnName("customer_name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(sale => sale.Branch, branch =>
        {
            branch.Property(b => b.Id)
                .HasColumnName("branch_id")
                .HasColumnType("uuid")
                .IsRequired();

            branch.Property(b => b.Name)
                .HasColumnName("branch_name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.HasMany(sale => sale.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(sale => sale.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

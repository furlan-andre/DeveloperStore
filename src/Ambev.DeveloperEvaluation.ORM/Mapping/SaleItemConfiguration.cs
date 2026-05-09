using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property<Guid>("SaleId")
            .HasColumnName("sale_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.OwnsOne(item => item.Product, product =>
        {
            product.Property(p => p.Id)
                .HasColumnName("product_id")
                .HasColumnType("uuid")
                .IsRequired();

            product.Property(p => p.Description)
                .HasColumnName("product_description")
                .HasMaxLength(300)
                .IsRequired();
        });

        builder.Property(item => item.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.Discount)
            .HasColumnName("discount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(item => item.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();
    }
}

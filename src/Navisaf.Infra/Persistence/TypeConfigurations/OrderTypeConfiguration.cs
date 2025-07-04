using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navisaf.Domain.Entities;

namespace Navisaf.Infra.Persistence.TypeConfigurations;

public class OrderTypeConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("Orders");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("Quantity")
            .IsRequired();

        builder.Property(x => x.CustomerName)
            .HasColumnName("CustomerName")
            .IsUnicode(false)
            .HasPrecision(100)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("Price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DistanceKilometers)
            .HasColumnName("DistanceKilometers")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.OrderDate)
            .HasColumnName("OrderDate")
            .IsRequired();

        builder.OwnsOne(x => x.Origin, origin =>
        {
            origin.Property(o => o.Latitude)
                .HasColumnName("OriginLatitude")
                .IsRequired();
            origin.Property(o => o.Longitude)
                .HasColumnName("OriginLongitude")
                .IsRequired();
        });

        builder.OwnsOne(x => x.Destination, destination =>
        {
            destination.Property(d => d.Latitude)
                .HasColumnName("DestinationLatitude")
                .IsRequired();
            destination.Property(d => d.Longitude)
                .HasColumnName("DestinationLongitude")
                .IsRequired();
        });

        builder.HasOne(x => x.Product)
            .WithMany(p => p.Orders)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_Orders_Products")
            .OnDelete(DeleteBehavior.Cascade);

    }
}
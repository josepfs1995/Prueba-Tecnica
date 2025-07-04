using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navisaf.Domain.Entities;

namespace Navisaf.Infra.Persistence.TypeConfigurations;

public class ProductTypeConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("Products");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsUnicode(false)
            .HasPrecision(100);
    }
}
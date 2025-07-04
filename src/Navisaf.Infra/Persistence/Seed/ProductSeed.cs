using Microsoft.EntityFrameworkCore;
using Navisaf.Domain.Entities;

namespace Navisaf.Infra.Persistence.Seed;

internal static class ProductSeed
{
    internal  static void SeedProducts(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("7f2e1f2e-41b2-4a0c-b709-5e1f6a66e921"),
                Name = "Laptop Dell"
            },
            new Product
            {
                Id = Guid.Parse("9c0a8cd2-2b88-4bc7-a239-6b876ba7e15c"),
                Name = "Iphone 16 Pro Max"
            },
            new Product
            {
                Id = Guid.Parse("2d17847e-d760-443c-bcf1-e6d4109b4972"),
                Name = "Disco Duro SSD 1TB"
            }
        );
    }
}
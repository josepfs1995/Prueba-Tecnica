using Microsoft.EntityFrameworkCore;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<Order> Orders  { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
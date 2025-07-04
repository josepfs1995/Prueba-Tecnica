using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Infra.Persistence;

namespace Navisaf.Application.Test;

public class InMemoryHelper
{
    public static IApplicationDbContext CreateNavisafContext()
    {
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<NavisafDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new NavisafDbContext(options);
        context.Database.EnsureDeleted(); // Ensure a clean state for tests
        context.Database.EnsureCreated(); // Create the database schema
        return context;
    }
}
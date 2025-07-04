using FluentAssertions;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Application.Features.Orders.Queries;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Test.Features.Orders.Queries;

public class OrderByIdQueryHandlerShould
{
    private readonly IApplicationDbContext _context = InMemoryHelper.CreateNavisafContext();
    [Fact]
    public async Task Throw_KeyNotFoundException_When_Order_Not_Found()
    {
        // Arrange
        var handler = new OrderByIdQueryHandler(_context);
        var query = new OrderByIdQuery(Guid.NewGuid());

        // Act
        var action = async () => await handler.Handle(query, CancellationToken.None);
        // Assert
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
    [Fact]
    public async Task Return_Expected_Data()
    {
        // Arrange
        var orders = GetOrders();
        _context.Orders.AddRange(orders);
        var orderId = orders.First().Id;
        await _context.SaveChangesAsync();

        var query = new OrderByIdQuery(orderId);
        var sut = new OrderByIdQueryHandler(_context);
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(orderId);
    }
    private static List<Order> GetOrders()
    {
        var orders = new Bogus.Faker<Order>()
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.ProductId, f => f.Random.Guid())
            .RuleFor(o => o.Product, f => new Product
            {
                Id = f.Random.Guid(),
                Name = f.Commerce.ProductName()
            })
            .RuleFor(o => o.Quantity, f => f.Random.Number(10, 100))
            .RuleFor(o => o.OrderDate, f => f.Date.Past(1))
            .RuleFor(o => o.Price, f => f.Finance.Amount(10, 1000))
            .RuleFor(o => o.CustomerName, f => f.Name.FullName())
            .RuleFor(o => o.Origin, f => new Location
            {
                Latitude = f.Address.Latitude(),
                Longitude = f.Address.Longitude(),
            })
            .RuleFor(o => o.Destination, f => new Location
            {
                Latitude = f.Address.Latitude(),
                Longitude = f.Address.Longitude(),
            })
            .Generate(10);
        return orders;
    }
}
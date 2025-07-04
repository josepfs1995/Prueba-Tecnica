using FluentAssertions;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Application.Features.Orders.Queries;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Test.Features.Orders.Queries;

public class UnitTest1
{
    private readonly IApplicationDbContext _context = InMemoryHelper.CreateNavisafContext();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Return_All_Data_If_CustomerName_Is_Null_Or_Empty(string? customerName)
    {
        //Arrange
        var orders = GetOrders();
        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();

        var query = new OrderListQuery(customerName);
        var sut = new OrderListQueryHandler(_context);

        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(orders.Count);
    }

    [Fact]
    public async Task Return_Expected_Data_If_CustomerName_Is_Present()
    {
        //Arrange
        var orders = GetOrders();
        _context.Orders.AddRange(orders);

        var customerName = orders.First().CustomerName;
        await _context.SaveChangesAsync();

        var query = new OrderListQuery(customerName);
        var sut = new OrderListQueryHandler(_context);
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(o => o.CustomerName == customerName);
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
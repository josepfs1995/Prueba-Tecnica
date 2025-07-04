using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Application.Features.Orders.Command;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Test.Features.Orders.Command;

public class CreateOrderCommandHandlerShould
{
    private readonly IApplicationDbContext _context = InMemoryHelper.CreateNavisafContext();
    [Fact]
    public async Task Throw_KeyNotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            CustomerName = "Test Customer"
        };

        var sut = new CreateOrderCommandHandler(_context);

        // Act
        var action = async () => await sut.Handle(command, CancellationToken.None);
        // Assert
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
    [Theory]
    [InlineData("-12.065311, -76.939984", "4.652469, -74.102156")] // Perú - Colombia
    [InlineData("-12.065311, -76.939984","-12.065311, -76.939984")] // Perú Lima Centro - Perú Lima Centro
    public async Task Throw_Exception_If_Distance_Is_Less_Than_One_Kilometer_Or_Is_Greater_Than_Thousand_Kilometers(string origin, string destination)
    {
        // Arrange
        var product = CreateProduct();
        _context.Products.Add(product);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand
        {
            ProductId =  product.Id,
            Quantity = 1,
            CustomerName = "Test Customer",
            OriginCoords = origin,
            DestinationCoords = destination
        };

        var sut = new CreateOrderCommandHandler(_context);

        // Act
        var action = async () => await sut.Handle(command, CancellationToken.None);
        // Assert
        await action.Should().ThrowAsync<Exception>();
    }
    [Fact]
    public async Task Return_Expected_Data()
    {
        // Arrange
        var product = CreateProduct();
        _context.Products.Add(product);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand
        {
            ProductId = product.Id,
            Quantity = 1,
            CustomerName = "Test Customer",
            OriginCoords = "-12.096818, -76.898979", // Perú - Himalayas
            DestinationCoords = "-12.065125, -76.940465" //Perú Fontana,
        };

        var sut = new CreateOrderCommandHandler(_context);

        // Act
        var result =  await sut.Handle(command, CancellationToken.None);
        // Assert
        result.Should().NotBeNull();

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.CustomerName == command.CustomerName && x.ProductId == command.ProductId, CancellationToken.None);
        order.Should().NotBeNull();
        order.Id.Should().Be(order.Id);
        order.OrderDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        order.Price.Should().Be(100);
        order.Quantity.Should().Be(command.Quantity);
        order.CustomerName.Should().Be(command.CustomerName);
        order.Origin.Latitude.Should().Be(-12.096818);
        order.Origin.Longitude.Should().Be(-76.898979);
        order.Destination.Latitude.Should().Be(-12.065125);
        order.Destination.Longitude.Should().Be(-76.940465);
        order.DistanceKilometers.Should().BeInRange(5, 7);
    }

    private static Product CreateProduct() =>  new Faker<Product>()
        .RuleFor(o => o.Id, f => f.Random.Guid())
        .RuleFor(o => o.Name, f => f.Commerce.ProductName())
        .Generate();
}
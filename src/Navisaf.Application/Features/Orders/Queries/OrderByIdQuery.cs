using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Features.Orders.Queries;

public sealed class OrderByIdQuery(Guid orderId) : IRequest<OrderByIdDto>
{
    public Guid OrderId { get; set; } = orderId;
}

public sealed class OrderByIdQueryHandler(IApplicationDbContext applicationDbContext): IRequestHandler<OrderByIdQuery, OrderByIdDto>
{
    public async Task<OrderByIdDto> Handle(OrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await applicationDbContext.Orders
            .Include(x => x.Product)
            .Where(x => x.Id == request.OrderId)
           .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException($"No se ha encontrado un pedido con el ID {request.OrderId}");
        }

        return MapToDto(order);
    }

    private static OrderByIdDto MapToDto(Order order)
    {
        return new OrderByIdDto
        {
            Id = order.Id,
            ProductName = order.Product.Name,
            Origin = $"{order.Origin.Latitude},{order.Origin.Longitude}",
            OrderDate = order.OrderDate,
            Destination = $"{order.Destination.Latitude},{order.Destination.Longitude}",
            Quantity = order.Quantity,
            CustomerName = order.CustomerName,
            Price = order.Price,
            DistanceKilometers = order.DistanceKilometers
        };
    }
}
public class OrderByIdDto
{
    public Guid Id  { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public decimal Price { get; set; }
    public double DistanceKilometers { get; set; }
}
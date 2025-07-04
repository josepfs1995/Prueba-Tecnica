using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Features.Orders.Queries;

public sealed class OrderListQuery(string? customerName) : IRequest<List<OrderDto>>
{
    public string? CustomerName { get; set; }  = customerName;
}

public sealed class OrderListQueryHandler(IApplicationDbContext applicationDbContext): IRequestHandler<OrderListQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(OrderListQuery request, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.Orders
            .Include(x => x.Product)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.CustomerName))
        {
            query = query.Where(o => o.CustomerName == request.CustomerName);
        }

        var orders = await query.ToListAsync(cancellationToken);
        var orderDtos = orders.Select(MapToDto).ToList();
        return orderDtos;
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            ProductName = order.Product.Name,
            Quantity = order.Quantity,
            CustomerName = order.CustomerName,
            Price = order.Price,
            DistanceKilometers = order.DistanceKilometers
        };
    }
}

public class OrderDto
{
    public Guid Id  { get; set; }
    public Guid ProductId   { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Price { get; set; }
    public double DistanceKilometers { get; set; }
}
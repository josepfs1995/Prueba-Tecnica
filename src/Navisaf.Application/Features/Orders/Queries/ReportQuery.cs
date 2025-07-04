using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;

namespace Navisaf.Application.Features.Orders.Queries;

public sealed class ReportQuery : IRequest<List<ReportDto>>;

public sealed class ReportQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<ReportQuery, List<ReportDto>>
{
    public async Task<List<ReportDto>> Handle(ReportQuery request, CancellationToken cancellationToken)
    {
        var orders = await applicationDbContext.Orders
            .Include(x => x.Product)
            .ToListAsync(cancellationToken);

        var groupedByPeriod = orders
            .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month, x.CustomerName })
            .Select(g => new ReportDto
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                TotalOrders = g.Count(),
                CustomerName = g.Key.CustomerName
            })
            .OrderByDescending(x => x.Period)
            .ToList();

        return groupedByPeriod;
    }
}

public class ReportDto
{
    public string Period { get; set; }
    public string CustomerName { get; set; }
    public int TotalOrders { get; set; }
}
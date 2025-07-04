using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Features.Products.Queries;

public sealed class ProductListQuery: IRequest<List<ProductDto>>;

public sealed class ProductListQueryHandler(IApplicationDbContext applicationDbContext): IRequestHandler<ProductListQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(ProductListQuery request, CancellationToken cancellationToken)
    {
        var response = await applicationDbContext.Products.ToListAsync(cancellationToken);
        var productDtos = response.Select(MapToDto).ToList();
        return productDtos;
    }

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name
    };
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
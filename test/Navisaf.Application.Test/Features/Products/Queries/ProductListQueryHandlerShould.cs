using FluentAssertions;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Application.Features.Products.Queries;

namespace Navisaf.Application.Test.Features.Products.Queries;

public class ProductListQueryHandlerShould
{
    private readonly IApplicationDbContext _context = InMemoryHelper.CreateNavisafContext();

    [Fact]
    public async Task Return_Expected_Data_If_CustomerName_Is_Present()
    {
        //Arrange
        var query = new ProductListQuery();
        var sut = new ProductListQueryHandler(_context);
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // Hay 3 por la semilla de EF
    }
}
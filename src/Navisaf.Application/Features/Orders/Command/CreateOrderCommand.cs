using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Helpers;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Domain.Entities;

namespace Navisaf.Application.Features.Orders.Command;

//HACK: Hacemos uso de Unit para que pueda usar ValidationBehavior
public sealed class CreateOrderCommand: IRequest<Unit>
{
    public Guid ProductId   { get; set; }
    public int Quantity { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public string OriginCoords { get; set; }
    public string DestinationCoords { get; set; }
    internal Location Origin => new(OriginCoords);
    internal Location Destination => new(DestinationCoords);
}

public sealed class CreateOrderCommandHandler(IApplicationDbContext applicationDbContext): IRequestHandler<CreateOrderCommand, Unit>
{
    public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var product = await applicationDbContext.Products
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el producto con ID {request.ProductId}. Por favor, verifique el ID del producto.");
        }

        var distance = CalculateDistance(request.Origin, request.Destination);
        var order = new Order
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            CustomerName = request.CustomerName,
            OrderDate = request.OrderDate,
            Origin = request.Origin,
            Destination = request.Destination,
            Price = CalculatePrice(distance),
            DistanceKilometers = distance
        };

        applicationDbContext.Orders.Add(order);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static double CalculateDistance(Location origin, Location destination)
    {
        var distance = GeoUtils.CalculateHaversineDistance(origin, destination);
        return distance switch
        {
            > 1000 => throw new Exception("Distancia demasiado larga, debe ser menor a 1000 km"),
            < 1 => throw new Exception("Distancia demasiado corta, debe ser mayor a 1 km"),
            _ => distance
        };
    }

    private static decimal CalculatePrice(double km) => km switch
    {
        < 50 => 100.00m,
        < 200 => 300.00m,
        < 500 => 1000.00m,
        < 1500 => 1500.00m,
        _ => throw new ArgumentOutOfRangeException(nameof(km), "Distancia fuera de rango para calcular el precio")
    };
}

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    private const string RequiredMessage = "{PropertyName} es requerido.";
    private const string InvalidNumberMessage = "{PropertyName} debe ser un numero valido.";

    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage(RequiredMessage);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("debe ser mayor a cero.");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage(RequiredMessage);
        RuleFor(x => x.OriginCoords).NotEmpty().WithMessage(RequiredMessage)
            .Must(IsValidCoords)
            .WithMessage(InvalidNumberMessage);

        RuleFor(x => x.DestinationCoords).NotEmpty().WithMessage(RequiredMessage)
            .Must(IsValidCoords)
            .WithMessage(InvalidNumberMessage);
    }

    private static bool IsValidCoords(string coords)
    {
        if (string.IsNullOrWhiteSpace(coords))
        {
            return false;
        }

        var parts = coords.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        return IsValidDouble(parts[0]) && IsValidDouble(parts[1]);
    }
   private static bool IsValidDouble(string coords) => double.TryParse(coords, out _);
}
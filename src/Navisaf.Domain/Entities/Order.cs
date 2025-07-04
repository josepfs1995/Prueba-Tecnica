namespace Navisaf.Domain.Entities;

public class Order
{
    public Guid Id  { get; set; } = Guid.NewGuid();
    public required Guid ProductId   { get; set; }
    public required int Quantity { get; set; }
    public required string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public required Location Origin { get; set; }
    public required Location Destination { get; set; }
    public required decimal Price { get; set; }
    public required double DistanceKilometers { get; set; }
    public Product Product { get; set; }
}
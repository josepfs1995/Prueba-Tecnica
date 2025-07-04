namespace Navisaf.Domain.Entities;

public class Location
{
    // HACK: Esto es necesario para Entity Framework
    public Location()
    {

    }
    public Location(string coords)
    {
        if (string.IsNullOrWhiteSpace(coords))
        {
            Latitude = 0;
            Longitude = 0;
            return;
        }
        var parts = coords.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            Latitude = 0;
            Longitude = 0;
            return;
        }
        var latitude = parts[0].Trim();
        var longitude = parts[1].Trim();

        Latitude = double.TryParse(latitude, out double lat) ? lat : 0;
        Longitude = double.TryParse(longitude, out double lon) ? lon : 0;
    }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
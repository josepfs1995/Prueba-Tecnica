using Navisaf.Domain.Entities;

namespace Navisaf.Application.Common.Helpers;

public class GeoUtils
{
    /// <summary>
    ///  Calculates the Haversine distance between two geographical locations.
    ///  </summary>
    public static double CalculateHaversineDistance(Location origin, Location destination)
    {
        // https://www.genbeta.com/desarrollo/como-calcular-la-distancia-entre-dos-puntos-geograficos-en-c-formula-de-haversine
        const double earthRadiusKm = 6371.0;

        var originLatitudeRadians = ToRadians(origin.Latitude);
        var originLongitudeRadians = ToRadians(origin.Longitude);
        var destinationLatitudeRadians = ToRadians(destination.Latitude);
        var destinationLongitudeRadians = ToRadians(destination.Longitude);

        var difLatitude = destinationLatitudeRadians - originLatitudeRadians;
        var difLongitude = destinationLongitudeRadians - originLongitudeRadians;

        var a = Math.Pow(Math.Sin(difLatitude / 2), 2) +
                   Math.Cos(originLatitudeRadians) * Math.Cos(destinationLatitudeRadians) *
                   Math.Pow(Math.Sin(difLongitude / 2), 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return Math.Round(earthRadiusKm * c, 2);
    }
    private static double ToRadians(double angle) => angle * Math.PI / 180.0;
}
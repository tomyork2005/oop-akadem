using System.Text.Json.Serialization;

namespace LogiCore.Domain.Routing;

public readonly struct RoutePoint
{
    public const double EarthRadiusKm = 6371.0;
    public const double DegreesInHalfCircle = 180.0;

    [JsonConstructor]
    public RoutePoint(string name, double latitude, double longitude)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
    }

    public string Name { get; }
    public double Latitude { get; }
    public double Longitude { get; }

    public static double operator -(RoutePoint a, RoutePoint b)
    {
        var lat1 = ToRadians(a.Latitude);
        var lat2 = ToRadians(b.Latitude);
        var dLat = ToRadians(b.Latitude - a.Latitude);
        var dLon = ToRadians(b.Longitude - a.Longitude);

        var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(h));
    }

    public static explicit operator string(RoutePoint point)
    {
        return point.ToString();
    }

    public override string ToString()
    {
        return $"{Name} ({Latitude}; {Longitude})";
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / DegreesInHalfCircle;
    }
}

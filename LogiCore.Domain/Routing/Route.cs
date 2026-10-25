using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Routing;

public class Route
{
    public const int MinPointCount = 2;
    public const int DistancePrecision = 2;

    private readonly List<RoutePoint> _points;

    public Route(IEnumerable<RoutePoint> points)
    {
        _points = new List<RoutePoint>(points);
        if (_points.Count < MinPointCount)
        {
            throw new ArgumentException($"в маршруте должно быть минимум {MinPointCount} точки");
        }
    }

    public IReadOnlyCollection<RoutePoint> Points
    {
        get { return _points.AsReadOnly(); }
    }

    public decimal DistanceKm
    {
        get
        {
            double total = 0;
            for (int i = 1; i < _points.Count; i++)
            {
                total += _points[i] - _points[i - 1];
            }
            return Math.Round((decimal)total, DistancePrecision);
        }
    }

    public TimeSpan EstimateTime(Vehicle vehicle)
    {
        return TimeSpan.FromHours((double)(DistanceKm / vehicle.AverageSpeedKmH));
    }

    public override string ToString()
    {
        var names = new List<string>();
        foreach (var point in _points)
        {
            names.Add(point.Name);
        }
        return string.Join(" → ", names) + $" ({DistanceKm} км)";
    }
}

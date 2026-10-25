using LogiCore.Domain.Routing;

namespace LogiCore.App;

public static class Cities
{
    public static IReadOnlyList<RoutePoint> All()
    {
        return new List<RoutePoint>
        {
            new RoutePoint("Москва", 55.7558, 37.6173),
            new RoutePoint("Химки", 55.8970, 37.4297),
            new RoutePoint("Зеленоград", 55.9825, 37.1814),
            new RoutePoint("Санкт-Петербург", 59.9343, 30.3351),
            new RoutePoint("Казань", 55.7963, 49.1088),
            new RoutePoint("Екатеринбург", 56.8389, 60.6057),
            new RoutePoint("Новосибирск", 55.0084, 82.9357),
            new RoutePoint("Краснодар", 45.0355, 38.9753),
            new RoutePoint("Калининград", 54.7104, 20.4522)
        };
    }

    public static RoutePoint Get(string name)
    {
        foreach (var city in All())
        {
            if (city.Name == name)
            {
                return city;
            }
        }
        throw new ArgumentException("нет такого города: " + name);
    }
}

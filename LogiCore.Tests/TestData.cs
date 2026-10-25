using LogiCore.Domain.Cargos;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Routing;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Tests;

internal static class TestData
{
    public const decimal EquatorDegreeKm = 111.19m;

    public static Route EquatorRoute()
    {
        return new Route(new[] { new RoutePoint("A", 0, 0), new RoutePoint("B", 0, 1) });
    }

    public static Route ZeroRoute()
    {
        return new Route(new[] { new RoutePoint("A", 55, 37), new RoutePoint("A", 55, 37) });
    }

    public static Truck Truck()
    {
        return new Truck(Guid.NewGuid(), "TRK-001", 10000m, 50m, 80m, 10m, 1.2m);
    }

    public static RefrigeratorTruck Reefer()
    {
        return new RefrigeratorTruck(Guid.NewGuid(), "REF-001", 8000m, 40m, 70m, 12m, 1.1m, -20m, 5m, 3m);
    }

    public static CargoPlane Plane()
    {
        return new CargoPlane(Guid.NewGuid(), "AIR-001", 30000m, 200m, 800m, 100m, 2m);
    }

    public static CargoShip Ship()
    {
        return new CargoShip(Guid.NewGuid(), "SEA-001", 100000m, 1000m, 30m, 5m, 1000m);
    }

    public static DroneCourier Drone()
    {
        return new DroneCourier(Guid.NewGuid(), "DRN-001", 10m, 0.1m, 60m, 20m, 30m, 100m);
    }

    public static StandardCargo Standard(decimal weightKg = 100m, decimal volumeM3 = 1m)
    {
        return new StandardCargo(Guid.NewGuid(), "Коробки", weightKg, volumeM3, 1000m);
    }

    public static PerishableCargo Perishable(decimal temperatureC = 4m, int expiresInDays = 5)
    {
        return new PerishableCargo(Guid.NewGuid(), "Молоко", 100m, 1m, 1000m, DateTime.Now.AddDays(expiresInDays), temperatureC);
    }

    public static FragileCargo Fragile()
    {
        return new FragileCargo(Guid.NewGuid(), "Стекло", 50m, 0.5m, 10000m, 0.5m);
    }

    public static DangerousCargo Dangerous(int hazardClass = 3)
    {
        return new DangerousCargo(Guid.NewGuid(), "Краска", 200m, 1m, 5000m, hazardClass);
    }

    public static OversizedCargo Oversized()
    {
        return new OversizedCargo(Guid.NewGuid(), "Турбина", 5000m, 40m, 100000m);
    }

    public static Order Order(params Cargo[] cargo)
    {
        var customer = new Customer(Guid.NewGuid(), "Клиент", "test@example.com");
        return new Order(Guid.NewGuid(), customer, cargo, EquatorRoute(), StandardTariff.TariffName, ExtraServices.None);
    }
}

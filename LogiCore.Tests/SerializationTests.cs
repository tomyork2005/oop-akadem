using LogiCore.Domain.Cargos;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Persistence;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Tests;

public class SerializationTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), "logicore-test-" + Guid.NewGuid().ToString("N") + ".json");
    private readonly JsonStateStorage _storage = new JsonStateStorage();

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    [Fact]
    public void RoundTrip_DataIsEquivalent()
    {
        var original = BuildState();

        _storage.Save(_path, original);
        var loaded = new CompanyData();
        string error;
        var ok = _storage.TryLoad(_path, loaded, out error);

        Assert.True(ok, error);
        Assert.Equal(original.Vehicles.Count, loaded.Vehicles.Count);
        Assert.Equal(original.CargoItems.Count, loaded.CargoItems.Count);
        Assert.Equal(original.Customers.Count, loaded.Customers.Count);
        Assert.Equal(original.Orders.Count, loaded.Orders.Count);
        Assert.Equal(original.Ledger.Total, loaded.Ledger.Total);

        foreach (var vehicle in original.Vehicles)
        {
            Assert.Equal(vehicle.GetType(), loaded.Vehicles[vehicle.Id].GetType());
            Assert.Equal(vehicle.State, loaded.Vehicles[vehicle.Id].State);
        }
        foreach (var cargo in original.CargoItems)
        {
            Assert.Equal(cargo.GetType(), loaded.CargoItems[cargo.Id].GetType());
        }
        foreach (var order in original.Orders)
        {
            Assert.Equal(order.Status, loaded.Orders[order.Id].Status);
            Assert.Equal(order.TotalCost, loaded.Orders[order.Id].TotalCost);
        }
    }

    [Fact]
    public void MissingFile_ReturnsFalse()
    {
        string error;

        Assert.False(_storage.TryLoad(_path, new CompanyData(), out error));
    }

    [Fact]
    public void CorruptedFile_ReturnsFalseAndKeepsData()
    {
        var data = BuildState();
        var count = data.Orders.Count;
        File.WriteAllText(_path, "{ это не json");
        string error;

        Assert.False(_storage.TryLoad(_path, data, out error));
        Assert.Equal(count, data.Orders.Count);
    }

    private static CompanyData BuildState()
    {
        var data = new CompanyData();
        var tariffs = new TariffRegistry();
        tariffs.Register(StandardTariff.TariffName, new StandardTariff());
        var service = new DeliveryService(data, new PricingService(tariffs));

        data.Vehicles.Add(TestData.Truck());
        data.Vehicles.Add(TestData.Reefer());
        data.Vehicles.Add(TestData.Plane());
        data.Vehicles.Add(TestData.Ship());
        data.Vehicles.Add(TestData.Drone());

        var delivered = TestData.Order(TestData.Standard());
        IDeliveryCost? cost;
        service.PlaceOrder(delivered);
        service.TryAssignBestVehicle(delivered, out cost);
        service.StartDelivery(delivered);
        service.CompleteDelivery(delivered);

        var inTransit = TestData.Order(TestData.Perishable(-10m));
        service.PlaceOrder(inTransit);
        service.TryAssignBestVehicle(inTransit, out cost);
        service.StartDelivery(inTransit);

        var assigned = TestData.Order(TestData.Fragile(), TestData.Dangerous());
        service.PlaceOrder(assigned);
        service.TryAssignBestVehicle(assigned, out cost);

        var cancelled = TestData.Order(TestData.Oversized());
        service.PlaceOrder(cancelled);
        service.CancelOrder(cancelled);

        return data;
    }
}

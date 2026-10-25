using System.Text.Json;
using LogiCore.Domain.Cargos;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Routing;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Persistence;

public class JsonStateStorage
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = true };

    public void Save(string path, CompanyData data)
    {
        var snapshot = ToSnapshot(data);
        using (var stream = File.Create(path))
        {
            JsonSerializer.Serialize(stream, snapshot, _options);
        }
    }

    public bool TryLoad(string path, CompanyData data, out string error)
    {
        error = "";
        if (!File.Exists(path))
        {
            error = "файл не найден: " + path;
            return false;
        }

        FileStream? stream = null;
        try
        {
            stream = File.OpenRead(path);
            var snapshot = JsonSerializer.Deserialize<SystemSnapshot>(stream, _options);
            if (snapshot == null)
            {
                error = "файл пустой";
                return false;
            }
            FromSnapshot(snapshot, data);
            return true;
        }
        catch (Exception ex) when (ex is JsonException || ex is NotSupportedException || ex is LogisticsException
                                   || ex is ArgumentException || ex is KeyNotFoundException)
        {
            error = "файл повреждён: " + ex.Message;
            return false;
        }
        finally
        {
            if (stream != null)
            {
                stream.Dispose();
            }
        }
    }

    private static SystemSnapshot ToSnapshot(CompanyData data)
    {
        var customers = new List<CustomerRecord>();
        foreach (var customer in data.Customers)
        {
            customers.Add(new CustomerRecord { Id = customer.Id, Name = customer.Name, Contact = customer.Contact });
        }

        var orders = new List<OrderRecord>();
        foreach (var order in data.Orders)
        {
            var cargoIds = new List<Guid>();
            foreach (var cargo in order.CargoItems)
            {
                cargoIds.Add(cargo.Id);
            }
            Guid? vehicleId = null;
            if (order.Vehicle != null)
            {
                vehicleId = order.Vehicle.Id;
            }
            orders.Add(new OrderRecord
            {
                Id = order.Id,
                CustomerId = order.Customer.Id,
                CargoIds = cargoIds,
                RoutePoints = new List<RoutePoint>(order.Route.Points),
                TariffName = order.TariffName,
                Services = order.Services,
                Status = order.Status,
                VehicleId = vehicleId,
                TotalCost = order.TotalCost
            });
        }

        return new SystemSnapshot
        {
            Vehicles = new List<Vehicle>(data.Vehicles),
            CargoItems = new List<Cargo>(data.CargoItems),
            Customers = customers,
            Orders = orders
        };
    }

    private static void FromSnapshot(SystemSnapshot snapshot, CompanyData data)
    {
        data.Clear();
        foreach (var vehicle in snapshot.Vehicles)
        {
            data.Vehicles.Add(vehicle);
        }
        foreach (var cargo in snapshot.CargoItems)
        {
            data.CargoItems.Add(cargo);
        }
        foreach (var record in snapshot.Customers)
        {
            data.Customers.Add(new Customer(record.Id, record.Name, record.Contact));
        }
        foreach (var record in snapshot.Orders)
        {
            var customer = data.Customers[record.CustomerId];
            var cargo = new List<Cargo>();
            foreach (var cargoId in record.CargoIds)
            {
                cargo.Add(data.CargoItems[cargoId]);
            }
            Vehicle? vehicle = null;
            if (record.VehicleId != null)
            {
                vehicle = data.Vehicles[record.VehicleId.Value];
            }

            var order = new Order(record.Id, customer, cargo, new Route(record.RoutePoints), record.TariffName, record.Services);
            order.Restore(record.Status, vehicle, record.TotalCost);
            data.Orders.Add(order);
            customer.AddOrder(order);
            if (order.Status == OrderStatus.Delivered && vehicle != null)
            {
                data.Ledger.Record(vehicle, order.TotalCost);
            }
        }
    }
}

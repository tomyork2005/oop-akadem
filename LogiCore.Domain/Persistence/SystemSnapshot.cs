using LogiCore.Domain.Cargos;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Routing;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Persistence;

public class SystemSnapshot
{
    public IReadOnlyCollection<Vehicle> Vehicles { get; init; } = new List<Vehicle>();
    public IReadOnlyCollection<Cargo> CargoItems { get; init; } = new List<Cargo>();
    public IReadOnlyCollection<CustomerRecord> Customers { get; init; } = new List<CustomerRecord>();
    public IReadOnlyCollection<OrderRecord> Orders { get; init; } = new List<OrderRecord>();
}

public class CustomerRecord
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Contact { get; init; } = "";
}

public class OrderRecord
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public IReadOnlyCollection<Guid> CargoIds { get; init; } = new List<Guid>();
    public IReadOnlyCollection<RoutePoint> RoutePoints { get; init; } = new List<RoutePoint>();
    public string TariffName { get; init; } = "";
    public ExtraServices Services { get; init; }
    public OrderStatus Status { get; init; }
    public Guid? VehicleId { get; init; }
    public decimal TotalCost { get; init; }
}

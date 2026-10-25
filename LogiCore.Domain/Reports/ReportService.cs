using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Reports;

public class ReportService
{
    public const decimal FullPercent = 100m;
    public const int Precision = 2;

    private readonly IReadOnlyRepository<Vehicle> _vehicles;
    private readonly IReadOnlyRepository<Cargo> _cargo;
    private readonly IReadOnlyRepository<Customer> _customers;
    private readonly IReadOnlyRepository<Order> _orders;
    private readonly RevenueLedger _ledger;

    public ReportService(IReadOnlyRepository<Vehicle> vehicles, IReadOnlyRepository<Cargo> cargo,
        IReadOnlyRepository<Customer> customers, IReadOnlyRepository<Order> orders, RevenueLedger ledger)
    {
        _vehicles = vehicles;
        _cargo = cargo;
        _customers = customers;
        _orders = orders;
        _ledger = ledger;
    }

    public IReadOnlyCollection<VehicleRevenueRow> TopVehiclesByRevenue(int count)
    {
        return _ledger.Entries
            .GroupBy(e => e.VehicleId)
            .Join(_vehicles.GetAll(), g => g.Key, v => v.Id,
                (g, v) => new VehicleRevenueRow(v.RegistrationNumber, v.GetType().Name, g.Sum(e => e.Amount)))
            .OrderByDescending(r => r.Revenue)
            .Take(count)
            .ToList();
    }

    public IReadOnlyCollection<StatusSummaryRow> OrdersByStatus()
    {
        var rows = from o in _orders.GetAll()
                   group o by o.Status into g
                   orderby g.Key
                   select new StatusSummaryRow(g.Key, g.Count(), g.Sum(x => x.TotalCost));
        return rows.ToList();
    }

    public IReadOnlyCollection<VehicleLoadRow> AverageLoadByVehicleType()
    {
        return _orders.GetAll()
            .Where(o => o.Vehicle != null && o.Status != OrderStatus.Cancelled)
            .Select(o => new { Type = VehicleTypeOf(o), Load = LoadPercentOf(o) })
            .GroupBy(x => x.Type)
            .Select(g => new VehicleLoadRow(g.Key, Math.Round(g.Average(x => x.Load), Precision)))
            .OrderByDescending(r => r.AverageLoadPercent)
            .ToList();
    }

    public IReadOnlyCollection<CustomerTotalRow> CustomersAbove(decimal threshold)
    {
        return _customers.GetAll()
            .Select(c => new CustomerTotalRow(c.Name, c.Orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalCost)))
            .Where(r => r.TotalAmount > threshold)
            .OrderByDescending(r => r.TotalAmount)
            .ToList();
    }

    public IReadOnlyCollection<CargoOrderCustomerRow> CargoOrderCustomer()
    {
        var lines = _orders.GetAll().SelectMany(o => o.CargoItems, (o, c) => new { CargoId = c.Id, Order = o });
        return _cargo.GetAll()
            .Join(lines, c => c.Id, l => l.CargoId, (c, l) => new { Cargo = c, l.Order })
            .Join(_customers.GetAll(), x => x.Order.Customer.Id, cu => cu.Id,
                (x, cu) => new CargoOrderCustomerRow(x.Cargo.Description, x.Order.Number, cu.Name))
            .OrderBy(r => r.CustomerName)
            .ToList();
    }

    public IReadOnlyDictionary<int, int> CargoCountByHazardClass()
    {
        return _cargo.GetAll()
            .OfType<DangerousCargo>()
            .GroupBy(d => d.HazardClass)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static string VehicleTypeOf(Order order)
    {
        if (order.Vehicle == null)
        {
            return "";
        }
        return order.Vehicle.GetType().Name;
    }

    private static decimal LoadPercentOf(Order order)
    {
        if (order.Vehicle == null)
        {
            return 0;
        }
        return order.TotalWeightKg / order.Vehicle.MaxLoadKg * FullPercent;
    }
}

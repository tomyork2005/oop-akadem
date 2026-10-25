using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Services;

public class CompanyData
{
    public Repository<Vehicle> Vehicles { get; } = new Repository<Vehicle>();
    public Repository<Cargo> CargoItems { get; } = new Repository<Cargo>();
    public Repository<Customer> Customers { get; } = new Repository<Customer>();
    public Repository<Order> Orders { get; } = new Repository<Order>();
    public RevenueLedger Ledger { get; } = new RevenueLedger();

    public void Clear()
    {
        Orders.Clear();
        Customers.Clear();
        CargoItems.Clear();
        Vehicles.Clear();
        Ledger.Clear();
    }
}

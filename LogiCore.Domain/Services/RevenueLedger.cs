using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Services;

public class RevenueEntry
{
    public RevenueEntry(Guid vehicleId, decimal amount)
    {
        VehicleId = vehicleId;
        Amount = amount;
    }

    public Guid VehicleId { get; }
    public decimal Amount { get; }
}

public class RevenueLedger
{
    private readonly List<RevenueEntry> _entries = new List<RevenueEntry>();
    private readonly object _sync = new object();

    public decimal Total
    {
        get
        {
            lock (_sync)
            {
                decimal sum = 0;
                foreach (var entry in _entries)
                {
                    sum += entry.Amount;
                }
                return sum;
            }
        }
    }

    public IReadOnlyCollection<RevenueEntry> Entries
    {
        get
        {
            lock (_sync)
            {
                return new List<RevenueEntry>(_entries);
            }
        }
    }

    public void Record(Vehicle vehicle, decimal amount)
    {
        lock (_sync)
        {
            _entries.Add(new RevenueEntry(vehicle.Id, amount));
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _entries.Clear();
        }
    }
}

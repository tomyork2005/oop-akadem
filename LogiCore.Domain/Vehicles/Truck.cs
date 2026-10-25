using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

public class Truck : Vehicle
{
    public const decimal DefaultTollRoadFactor = 1.15m;

    public Truck(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm,
        decimal tollRoadFactor)
        : base(id, registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        TollRoadFactor = tollRoadFactor;
    }

    public decimal TollRoadFactor { get; }

    public override TransportConditions Conditions
    {
        get { return TransportConditions.Sealed | TransportConditions.LongRange; }
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        return DistanceCost(route) * TollRoadFactor;
    }
}

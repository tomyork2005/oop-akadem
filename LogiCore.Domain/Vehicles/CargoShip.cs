using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

public class CargoShip : Vehicle
{
    public const decimal DefaultOversizedSurcharge = 3000m;

    public CargoShip(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm,
        decimal oversizedSurcharge)
        : base(id, registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        OversizedSurcharge = oversizedSurcharge;
    }

    public decimal OversizedSurcharge { get; }

    public override TransportConditions Conditions
    {
        get { return TransportConditions.Sealed | TransportConditions.LongRange; }
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        int oversizedCount = 0;
        foreach (var item in cargo)
        {
            if (item is OversizedCargo)
            {
                oversizedCount++;
            }
        }
        return DistanceCost(route) + oversizedCount * OversizedSurcharge;
    }
}

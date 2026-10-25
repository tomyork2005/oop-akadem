using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

public class CargoPlane : Vehicle
{
    public const decimal DefaultPerKgSurcharge = 2m;
    public const int ExplosivesHazardClass = 1;
    public const int RadioactiveHazardClass = 7;

    public CargoPlane(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm,
        decimal perKgSurcharge)
        : base(id, registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        PerKgSurcharge = perKgSurcharge;
    }

    public decimal PerKgSurcharge { get; }

    public override TransportConditions Conditions
    {
        get { return TransportConditions.Pressurized | TransportConditions.Sealed | TransportConditions.LongRange; }
    }

    public override bool CanCarry(Cargo cargo)
    {
        if (!base.CanCarry(cargo))
        {
            return false;
        }
        var dangerous = cargo as DangerousCargo;
        if (dangerous == null)
        {
            return true;
        }
        return dangerous.HazardClass != ExplosivesHazardClass && dangerous.HazardClass != RadioactiveHazardClass;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        return DistanceCost(route) + CargoTotals.Weight(cargo) * PerKgSurcharge;
    }
}

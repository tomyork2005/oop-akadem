using LogiCore.Domain.Cargos;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

public sealed class DroneCourier : Vehicle
{
    public const decimal HardWeightLimitKg = 20m;
    public const decimal DefaultMaxRangeKm = 40m;
    public const decimal DefaultLaunchFee = 150m;

    public DroneCourier(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm,
        decimal maxRangeKm, decimal launchFee)
        : base(id, registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        if (maxLoadKg > HardWeightLimitKg)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLoadKg), $"дрон не может поднимать больше {HardWeightLimitKg} кг");
        }
        MaxRangeKm = maxRangeKm;
        LaunchFee = launchFee;
    }

    public decimal MaxRangeKm { get; }
    public decimal LaunchFee { get; }

    public override bool CanTravel(Route route)
    {
        return route.DistanceKm <= MaxRangeKm;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        if (!CanTravel(route))
        {
            throw new RouteNotFoundException($"дрон {RegistrationNumber} не долетит: {route.DistanceKm} км при дальности {MaxRangeKm} км");
        }
        return LaunchFee + DistanceCost(route);
    }
}

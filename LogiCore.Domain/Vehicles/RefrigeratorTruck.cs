using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

public class RefrigeratorTruck : Truck
{
    public const decimal DefaultMinTemperatureC = -25m;
    public const decimal DefaultMaxTemperatureC = 8m;
    public const decimal DefaultCoolingRatePerKm = 4m;

    public RefrigeratorTruck(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm,
        decimal tollRoadFactor, decimal minTemperatureC, decimal maxTemperatureC, decimal coolingRatePerKm)
        : base(id, registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm, tollRoadFactor)
    {
        MinTemperatureC = minTemperatureC;
        MaxTemperatureC = maxTemperatureC;
        CoolingRatePerKm = coolingRatePerKm;
    }

    public decimal MinTemperatureC { get; }
    public decimal MaxTemperatureC { get; }
    public decimal CoolingRatePerKm { get; }

    public override TransportConditions Conditions
    {
        get { return base.Conditions | TransportConditions.Refrigerated; }
    }

    public bool SupportsTemperature(decimal temperatureC)
    {
        return temperatureC >= MinTemperatureC && temperatureC <= MaxTemperatureC;
    }

    public override bool CanCarry(Cargo cargo)
    {
        if (!base.CanCarry(cargo))
        {
            return false;
        }
        var sensitive = cargo as ITemperatureSensitive;
        if (sensitive == null)
        {
            return true;
        }
        return SupportsTemperature(sensitive.RequiredTemperatureC);
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        return base.CalculateDeliveryCost(route, cargo) + route.DistanceKm * CoolingRatePerKm;
    }
}

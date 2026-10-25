using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Factories;

public class VehicleFactory
{
    public const string TruckKind = "truck";
    public const string RefrigeratorKind = "refrigerator";
    public const string PlaneKind = "plane";
    public const string ShipKind = "ship";
    public const string DroneKind = "drone";

    public const string TollParameter = "toll";
    public const string MinTemperatureParameter = "minTemp";
    public const string MaxTemperatureParameter = "maxTemp";
    public const string CoolingRateParameter = "coolingRate";
    public const string PerKgSurchargeParameter = "perKg";
    public const string OversizedSurchargeParameter = "oversized";
    public const string MaxRangeParameter = "maxRange";
    public const string LaunchFeeParameter = "launchFee";

    public Vehicle Create(VehicleSpec s)
    {
        var id = Guid.NewGuid();
        switch (s.Kind)
        {
            case TruckKind:
                return new Truck(id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.AverageSpeedKmH, s.BaseRatePerKm,
                    s.Get(TollParameter, Truck.DefaultTollRoadFactor));
            case RefrigeratorKind:
                return new RefrigeratorTruck(id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.AverageSpeedKmH, s.BaseRatePerKm,
                    s.Get(TollParameter, Truck.DefaultTollRoadFactor),
                    s.Get(MinTemperatureParameter, RefrigeratorTruck.DefaultMinTemperatureC),
                    s.Get(MaxTemperatureParameter, RefrigeratorTruck.DefaultMaxTemperatureC),
                    s.Get(CoolingRateParameter, RefrigeratorTruck.DefaultCoolingRatePerKm));
            case PlaneKind:
                return new CargoPlane(id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.AverageSpeedKmH, s.BaseRatePerKm,
                    s.Get(PerKgSurchargeParameter, CargoPlane.DefaultPerKgSurcharge));
            case ShipKind:
                return new CargoShip(id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.AverageSpeedKmH, s.BaseRatePerKm,
                    s.Get(OversizedSurchargeParameter, CargoShip.DefaultOversizedSurcharge));
            case DroneKind:
                return new DroneCourier(id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.AverageSpeedKmH, s.BaseRatePerKm,
                    s.Get(MaxRangeParameter, DroneCourier.DefaultMaxRangeKm),
                    s.Get(LaunchFeeParameter, DroneCourier.DefaultLaunchFee));
            default:
                throw new ArgumentException($"неизвестный тип транспорта «{s.Kind}»");
        }
    }
}

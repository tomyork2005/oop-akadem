using LogiCore.Domain.Cargos;
using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Factories;

public class CargoFactory
{
    public const string StandardKind = "standard";
    public const string PerishableKind = "perishable";
    public const string FragileKind = "fragile";
    public const string DangerousKind = "dangerous";
    public const string OversizedKind = "oversized";

    public const string ExpiresInDaysParameter = "expiresInDays";
    public const string TemperatureParameter = "temperature";
    public const string RiskParameter = "risk";
    public const string HazardClassParameter = "hazardClass";

    public const decimal DefaultExpiresInDays = 7m;
    public const decimal DefaultTemperatureC = 4m;
    public const decimal DefaultHazardClass = 3m;

    public Cargo Create(CargoSpec s)
    {
        var id = Guid.NewGuid();
        switch (s.Kind)
        {
            case StandardKind:
                return new StandardCargo(id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue);
            case PerishableKind:
                var expiration = DateTime.Today.AddDays((double)s.Get(ExpiresInDaysParameter, DefaultExpiresInDays));
                return new PerishableCargo(id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue,
                    expiration, s.Get(TemperatureParameter, DefaultTemperatureC));
            case FragileKind:
                return new FragileCargo(id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue,
                    s.Get(RiskParameter, FragileCargo.DefaultRiskFactor));
            case DangerousKind:
                return new DangerousCargo(id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue,
                    (int)s.Get(HazardClassParameter, DefaultHazardClass));
            case OversizedKind:
                return new OversizedCargo(id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue);
            default:
                throw new CargoValidationException($"неизвестный тип груза «{s.Kind}»");
        }
    }
}

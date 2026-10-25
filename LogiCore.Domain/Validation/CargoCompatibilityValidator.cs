using LogiCore.Domain.Cargos;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Validation;

public class CargoCompatibilityValidator
{
    public ValidationResult Validate(Vehicle? vehicle, IReadOnlyCollection<Cargo> cargo, DateTime now)
    {
        var errors = new List<string>();
        CheckDangerousWithPerishable(cargo, errors);
        if (vehicle != null)
        {
            CheckTemperature(vehicle, cargo, errors);
            CheckCapacity(vehicle, cargo, errors);
        }
        CheckExpiration(cargo, now, errors);
        return new ValidationResult(errors);
    }

    private static void CheckDangerousWithPerishable(IReadOnlyCollection<Cargo> cargo, List<string> errors)
    {
        var hasDangerous = false;
        var hasPerishable = false;
        foreach (var item in cargo)
        {
            if (item is DangerousCargo)
            {
                hasDangerous = true;
            }
            if (item is PerishableCargo)
            {
                hasPerishable = true;
            }
        }
        if (hasDangerous && hasPerishable)
        {
            errors.Add("опасные грузы нельзя перевозить вместе со скоропортящимися");
        }
    }

    private static void CheckTemperature(Vehicle vehicle, IReadOnlyCollection<Cargo> cargo, List<string> errors)
    {
        var refrigerator = vehicle as RefrigeratorTruck;
        foreach (var item in cargo)
        {
            var sensitive = item as ITemperatureSensitive;
            if (sensitive == null)
            {
                continue;
            }
            if (refrigerator == null || !refrigerator.SupportsTemperature(sensitive.RequiredTemperatureC))
            {
                errors.Add($"{vehicle.RegistrationNumber} не держит {sensitive.RequiredTemperatureC}°C для «{item.Description}»");
            }
        }
    }

    private static void CheckCapacity(Vehicle vehicle, IReadOnlyCollection<Cargo> cargo, List<string> errors)
    {
        if (vehicle.IsOverloadedBy(cargo))
        {
            errors.Add($"перегруз {vehicle.RegistrationNumber}: {CargoTotals.Weight(cargo)} из {vehicle.MaxLoadKg} кг, " +
                       $"{CargoTotals.Volume(cargo)} из {vehicle.MaxVolumeM3} м³");
        }
    }

    private static void CheckExpiration(IReadOnlyCollection<Cargo> cargo, DateTime now, List<string> errors)
    {
        var validator = new CargoExpirationValidator(now);
        foreach (var item in cargo)
        {
            errors.AddRange(validator.Validate(item).Errors);
        }
    }
}

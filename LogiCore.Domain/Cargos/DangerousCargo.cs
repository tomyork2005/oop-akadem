using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Cargos;

public class DangerousCargo : Cargo
{
    public const int MinHazardClass = 1;
    public const int MaxHazardClass = 9;

    public DangerousCargo(Guid id, string description, decimal weightKg, decimal volumeM3, decimal declaredValue, int hazardClass)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        if (hazardClass < MinHazardClass || hazardClass > MaxHazardClass)
        {
            throw new CargoValidationException($"класс опасности должен быть от {MinHazardClass} до {MaxHazardClass}");
        }
        HazardClass = hazardClass;
    }

    public int HazardClass { get; }

    public override string Category
    {
        get { return "Опасный"; }
    }
}

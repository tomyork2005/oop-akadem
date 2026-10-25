using System.Text.Json.Serialization;
using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Cargos;

[JsonDerivedType(typeof(StandardCargo), "StandardCargo")]
[JsonDerivedType(typeof(PerishableCargo), "PerishableCargo")]
[JsonDerivedType(typeof(FragileCargo), "FragileCargo")]
[JsonDerivedType(typeof(DangerousCargo), "DangerousCargo")]
[JsonDerivedType(typeof(OversizedCargo), "OversizedCargo")]
public abstract class Cargo : IEntity
{
    protected Cargo(Guid id, string description, decimal weightKg, decimal volumeM3, decimal declaredValue)
    {
        if (weightKg <= 0)
        {
            throw new CargoValidationException($"вес груза «{description}» должен быть больше нуля");
        }
        if (volumeM3 <= 0)
        {
            throw new CargoValidationException($"объём груза «{description}» должен быть больше нуля");
        }

        Id = id;
        Description = description;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        DeclaredValue = declaredValue;
    }

    public Guid Id { get; }
    public string Description { get; }
    public decimal WeightKg { get; }
    public decimal VolumeM3 { get; }
    public decimal DeclaredValue { get; }

    public abstract string Category { get; }

    public virtual bool IsExpired(DateTime now)
    {
        return false;
    }

    public override string ToString()
    {
        return $"{Category} «{Description}» {WeightKg} кг / {VolumeM3} м³, {DeclaredValue:N0} руб.";
    }
}

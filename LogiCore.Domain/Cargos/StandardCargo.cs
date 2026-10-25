namespace LogiCore.Domain.Cargos;

public class StandardCargo : Cargo
{
    public StandardCargo(Guid id, string description, decimal weightKg, decimal volumeM3, decimal declaredValue)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
    }

    public override string Category
    {
        get { return "Обычный"; }
    }
}

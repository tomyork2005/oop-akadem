namespace LogiCore.Domain.Cargos;

public class PerishableCargo : Cargo, ITemperatureSensitive
{
    public PerishableCargo(Guid id, string description, decimal weightKg, decimal volumeM3, decimal declaredValue,
        DateTime expirationDate, decimal requiredTemperatureC)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        ExpirationDate = expirationDate;
        RequiredTemperatureC = requiredTemperatureC;
    }

    public DateTime ExpirationDate { get; }
    public decimal RequiredTemperatureC { get; }

    public override string Category
    {
        get { return "Скоропортящийся"; }
    }

    public override bool IsExpired(DateTime now)
    {
        return ExpirationDate < now;
    }
}

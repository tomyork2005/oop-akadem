namespace LogiCore.Domain.Factories;

public class CargoSpec
{
    private readonly Dictionary<string, decimal> _parameters = new Dictionary<string, decimal>();

    public CargoSpec(string kind, string description, decimal weightKg, decimal volumeM3, decimal declaredValue)
    {
        Kind = kind;
        Description = description;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        DeclaredValue = declaredValue;
    }

    public string Kind { get; }
    public string Description { get; }
    public decimal WeightKg { get; }
    public decimal VolumeM3 { get; }
    public decimal DeclaredValue { get; }

    public CargoSpec With(string name, decimal value)
    {
        _parameters[name] = value;
        return this;
    }

    public decimal Get(string name, decimal defaultValue)
    {
        decimal value;
        if (_parameters.TryGetValue(name, out value))
        {
            return value;
        }
        return defaultValue;
    }
}

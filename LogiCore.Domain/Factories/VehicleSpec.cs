namespace LogiCore.Domain.Factories;

public class VehicleSpec
{
    private readonly Dictionary<string, decimal> _parameters = new Dictionary<string, decimal>();

    public VehicleSpec(string kind, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm)
    {
        Kind = kind;
        RegistrationNumber = registrationNumber;
        MaxLoadKg = maxLoadKg;
        MaxVolumeM3 = maxVolumeM3;
        AverageSpeedKmH = averageSpeedKmH;
        BaseRatePerKm = baseRatePerKm;
    }

    public string Kind { get; }
    public string RegistrationNumber { get; }
    public decimal MaxLoadKg { get; }
    public decimal MaxVolumeM3 { get; }
    public decimal AverageSpeedKmH { get; }
    public decimal BaseRatePerKm { get; }

    public VehicleSpec With(string name, decimal value)
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

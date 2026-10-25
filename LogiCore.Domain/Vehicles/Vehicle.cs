using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Vehicles;

[JsonDerivedType(typeof(Truck), "Truck")]
[JsonDerivedType(typeof(RefrigeratorTruck), "RefrigeratorTruck")]
[JsonDerivedType(typeof(CargoPlane), "CargoPlane")]
[JsonDerivedType(typeof(CargoShip), "CargoShip")]
[JsonDerivedType(typeof(DroneCourier), "DroneCourier")]
public abstract class Vehicle : IEntity
{
    public const string RegistrationNumberPattern = "^[A-Z]{2,3}-[0-9]{3,4}$";

    private string _registrationNumber = "";

    protected Vehicle(Guid id, string registrationNumber, decimal maxLoadKg, decimal maxVolumeM3, decimal averageSpeedKmH, decimal baseRatePerKm)
    {
        if (maxLoadKg <= 0 || maxVolumeM3 <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLoadKg), "грузоподъёмность и объём должны быть больше нуля");
        }
        if (averageSpeedKmH <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(averageSpeedKmH), "скорость должна быть больше нуля");
        }

        Id = id;
        RegistrationNumber = registrationNumber;
        MaxLoadKg = maxLoadKg;
        MaxVolumeM3 = maxVolumeM3;
        AverageSpeedKmH = averageSpeedKmH;
        BaseRatePerKm = baseRatePerKm;
        State = VehicleState.Free;
    }

    public Guid Id { get; }

    public string RegistrationNumber
    {
        get { return _registrationNumber; }
        private set
        {
            if (value == null || !Regex.IsMatch(value, RegistrationNumberPattern))
            {
                throw new ArgumentException($"неверный формат номера «{value}», нужен вид AB-123 или ABC-1234");
            }
            _registrationNumber = value;
        }
    }

    public decimal MaxLoadKg { get; }
    public decimal MaxVolumeM3 { get; }
    public decimal AverageSpeedKmH { get; }
    public decimal BaseRatePerKm { get; }

    [JsonInclude]
    public VehicleState State { get; private set; }

    public virtual TransportConditions Conditions
    {
        get { return TransportConditions.None; }
    }

    public abstract decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo);

    public virtual bool CanCarry(Cargo cargo)
    {
        return cargo.WeightKg <= MaxLoadKg && cargo.VolumeM3 <= MaxVolumeM3;
    }

    public virtual bool CanTravel(Route route)
    {
        return true;
    }

    public bool IsOverloadedBy(IEnumerable<Cargo> cargo)
    {
        return CargoTotals.Weight(cargo) > MaxLoadKg || CargoTotals.Volume(cargo) > MaxVolumeM3;
    }

    public void Dispatch()
    {
        if (State != VehicleState.Free)
        {
            throw new InvalidOperationException($"{RegistrationNumber} сейчас не свободен");
        }
        State = VehicleState.InTransit;
    }

    public void Release()
    {
        State = VehicleState.Free;
    }

    protected decimal DistanceCost(Route route)
    {
        return route.DistanceKm * BaseRatePerKm;
    }

    public override string ToString()
    {
        return $"{GetType().Name} {RegistrationNumber} [{MaxLoadKg} кг, {MaxVolumeM3} м³, {State}]";
    }

    public override bool Equals(object? obj)
    {
        var other = obj as Vehicle;
        if (other == null)
        {
            return false;
        }
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

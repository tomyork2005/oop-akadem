using LogiCore.Domain.Orders;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Exceptions;

public abstract class LogisticsException : Exception
{
    protected LogisticsException(string message) : base(message)
    {
    }
}

public class CargoValidationException : LogisticsException
{
    public CargoValidationException(string message) : base(message)
    {
    }
}

public class IncompatibleCargoException : LogisticsException
{
    private readonly List<string> _violations;

    public IncompatibleCargoException(IReadOnlyCollection<string> violations)
        : base("грузы несовместимы: " + string.Join("; ", violations))
    {
        _violations = new List<string>(violations);
    }

    public IReadOnlyCollection<string> Violations
    {
        get { return _violations.AsReadOnly(); }
    }
}

public class VehicleOverloadException : LogisticsException
{
    public VehicleOverloadException(Vehicle vehicle, decimal requestedWeightKg, decimal requestedVolumeM3)
        : base($"перегруз {vehicle.RegistrationNumber}: {requestedWeightKg} из {vehicle.MaxLoadKg} кг, {requestedVolumeM3} из {vehicle.MaxVolumeM3} м³")
    {
        RequestedWeightKg = requestedWeightKg;
        MaxWeightKg = vehicle.MaxLoadKg;
    }

    public decimal RequestedWeightKg { get; }
    public decimal MaxWeightKg { get; }
}

public class RouteNotFoundException : LogisticsException
{
    public RouteNotFoundException(string message) : base(message)
    {
    }
}

public class InvalidOrderStateException : LogisticsException
{
    public InvalidOrderStateException(Guid orderId, OrderStatus currentStatus, OrderStatus requestedStatus)
        : base($"заказ {orderId}: переход {currentStatus} -> {requestedStatus} запрещён")
    {
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }

    public OrderStatus CurrentStatus { get; }
    public OrderStatus RequestedStatus { get; }
}

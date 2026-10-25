using LogiCore.Domain.Events;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Validation;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Services;

public class DeliveryService
{
    private readonly CompanyData _data;
    private readonly PricingService _pricing;
    private readonly CargoCompatibilityValidator _validator = new CargoCompatibilityValidator();

    public DeliveryService(CompanyData data, PricingService pricing)
    {
        _data = data;
        _pricing = pricing;
    }

    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    public event OrderStatusChangedHandler? OrderStatusChanged;
    public event EventHandler<VehicleOverloadAttemptEventArgs>? VehicleOverloadAttempt;
    public event EventHandler<DeliveryCompletedEventArgs>? DeliveryCompleted;

    public decimal TotalRevenue
    {
        get { return _data.Ledger.Total; }
    }

    public void PlaceOrder(Order order)
    {
        var result = _validator.Validate(null, order.CargoItems, DateTime.Now);
        if (!result.IsValid)
        {
            throw new IncompatibleCargoException(result.Errors);
        }

        foreach (var cargo in order.CargoItems)
        {
            if (_data.CargoItems.GetById(cargo.Id) == null)
            {
                _data.CargoItems.Add(cargo);
            }
        }
        if (_data.Customers.GetById(order.Customer.Id) == null)
        {
            _data.Customers.Add(order.Customer);
        }
        _data.Orders.Add(order);
        order.Customer.AddOrder(order);
        OnOrderCreated(new OrderCreatedEventArgs(order));
    }

    public bool TryAssignBestVehicle(Order order, out IDeliveryCost? cost)
    {
        cost = null;
        Vehicle? best = null;
        decimal bestCost = 0;
        foreach (var vehicle in _data.Vehicles)
        {
            if (CheckVehicle(vehicle, order) != null)
            {
                continue;
            }
            var vehicleCost = vehicle.CalculateDeliveryCost(order.Route, order.CargoItems);
            if (best == null || vehicleCost < bestCost)
            {
                best = vehicle;
                bestCost = vehicleCost;
            }
        }

        if (best == null)
        {
            return false;
        }
        cost = Assign(order, best);
        return true;
    }

    public IDeliveryCost AssignVehicle(Order order, Vehicle vehicle)
    {
        if (vehicle.IsOverloadedBy(order.CargoItems))
        {
            OnVehicleOverloadAttempt(new VehicleOverloadAttemptEventArgs(vehicle, order));
            throw new VehicleOverloadException(vehicle, order.TotalWeightKg, order.TotalVolumeM3);
        }
        var problem = CheckVehicle(vehicle, order);
        if (problem != null)
        {
            throw new IncompatibleCargoException(new List<string> { problem });
        }
        return Assign(order, vehicle);
    }

    public void StartDelivery(Order order)
    {
        var oldStatus = order.Status;
        order.StartDelivery();
        OnOrderStatusChanged(new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }

    public void CompleteDelivery(Order order)
    {
        var oldStatus = order.Status;
        order.Complete();
        if (order.Vehicle != null)
        {
            order.Vehicle.Release();
            _data.Ledger.Record(order.Vehicle, order.TotalCost);
        }
        OnOrderStatusChanged(new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
        OnDeliveryCompleted(new DeliveryCompletedEventArgs(order));
    }

    public void CancelOrder(Order order)
    {
        var oldStatus = order.Status;
        order.Cancel();
        if (order.Vehicle != null)
        {
            order.Vehicle.Release();
        }
        OnOrderStatusChanged(new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }

    public void InterruptDelivery(Order order)
    {
        var oldStatus = order.Status;
        order.Interrupt();
        if (order.Vehicle != null)
        {
            order.Vehicle.Release();
        }
        OnOrderStatusChanged(new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }

    protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
    {
        var handler = OrderCreated;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected virtual void OnOrderStatusChanged(OrderStatusChangedEventArgs e)
    {
        var handler = OrderStatusChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected virtual void OnVehicleOverloadAttempt(VehicleOverloadAttemptEventArgs e)
    {
        var handler = VehicleOverloadAttempt;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected virtual void OnDeliveryCompleted(DeliveryCompletedEventArgs e)
    {
        var handler = DeliveryCompleted;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    private IDeliveryCost Assign(Order order, Vehicle vehicle)
    {
        var cost = _pricing.Calculate(order, vehicle);
        var oldStatus = order.Status;
        vehicle.Dispatch();
        try
        {
            order.Assign(vehicle, Math.Round(cost.Total, TariffSettings.MoneyPrecision));
        }
        catch (InvalidOrderStateException)
        {
            vehicle.Release();
            throw;
        }
        OnOrderStatusChanged(new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
        return cost;
    }

    private string? CheckVehicle(Vehicle vehicle, Order order)
    {
        if (vehicle.State != VehicleState.Free)
        {
            return $"{vehicle.RegistrationNumber} занят";
        }
        if (!vehicle.CanTravel(order.Route))
        {
            return $"{vehicle.RegistrationNumber} не может пройти этот маршрут";
        }
        foreach (var cargo in order.CargoItems)
        {
            if (!vehicle.CanCarry(cargo))
            {
                return $"{vehicle.RegistrationNumber} не может везти «{cargo.Description}»";
            }
        }
        var result = _validator.Validate(vehicle, order.CargoItems, DateTime.Now);
        if (!result.IsValid)
        {
            return result.ToString();
        }
        return null;
    }
}

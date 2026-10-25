using LogiCore.Domain.Orders;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Events;

public class OrderCreatedEventArgs : EventArgs
{
    public OrderCreatedEventArgs(Order order)
    {
        Order = order;
    }

    public Order Order { get; }
}

public class OrderStatusChangedEventArgs : EventArgs
{
    public OrderStatusChangedEventArgs(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Order = order;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public Order Order { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
}

public class VehicleOverloadAttemptEventArgs : EventArgs
{
    public VehicleOverloadAttemptEventArgs(Vehicle vehicle, Order order)
    {
        Vehicle = vehicle;
        Order = order;
    }

    public Vehicle Vehicle { get; }
    public Order Order { get; }
}

public class DeliveryCompletedEventArgs : EventArgs
{
    public DeliveryCompletedEventArgs(Order order)
    {
        Order = order;
    }

    public Order Order { get; }
}

public class DeliveryProgressEventArgs : EventArgs
{
    public DeliveryProgressEventArgs(Order order, int percent)
    {
        Order = order;
        Percent = percent;
    }

    public Order Order { get; }
    public int Percent { get; }
}

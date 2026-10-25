namespace LogiCore.Domain.Orders;

public enum OrderStatus
{
    Created,
    Assigned,
    InTransit,
    Delivered,
    Cancelled,
    Interrupted
}

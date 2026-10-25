using LogiCore.Domain.Orders;

namespace LogiCore.Domain.Simulation;

internal class ActiveDelivery
{
    public ActiveDelivery(Order order, CancellationTokenSource cancellation, Task task)
    {
        Order = order;
        Cancellation = cancellation;
        Task = task;
    }

    public Order Order { get; }
    public CancellationTokenSource Cancellation { get; }
    public Task Task { get; }
}

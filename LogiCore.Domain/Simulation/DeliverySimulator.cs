using System.Collections.Concurrent;
using LogiCore.Domain.Events;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Services;

namespace LogiCore.Domain.Simulation;

public class DeliverySimulator
{
    public const int ProgressSteps = 5;
    public const int FullPercent = 100;

    private readonly DeliveryService _service;
    private readonly ConcurrentDictionary<Guid, ActiveDelivery> _active = new ConcurrentDictionary<Guid, ActiveDelivery>();

    public DeliverySimulator(DeliveryService service)
    {
        _service = service;
    }

    public event DeliveryProgressHandler? ProgressChanged;

    public IReadOnlyCollection<Order> ActiveOrders
    {
        get
        {
            var orders = new List<Order>();
            foreach (var pair in _active)
            {
                orders.Add(pair.Value.Order);
            }
            return orders;
        }
    }

    public Task Start(Order order, TimeSpan stepDelay)
    {
        _service.StartDelivery(order);
        var cancellation = new CancellationTokenSource();
        var task = new Task(() => Run(order, stepDelay, cancellation.Token), TaskCreationOptions.LongRunning);
        _active[order.Id] = new ActiveDelivery(order, cancellation, task);
        task.Start();
        return task;
    }

    public bool Cancel(Guid orderId)
    {
        ActiveDelivery? delivery;
        if (!_active.TryGetValue(orderId, out delivery))
        {
            return false;
        }
        delivery.Cancellation.Cancel();
        return true;
    }

    public void CancelAll()
    {
        foreach (var pair in _active)
        {
            pair.Value.Cancellation.Cancel();
        }
    }

    public void WaitAll(TimeSpan timeout)
    {
        var tasks = new List<Task>();
        foreach (var pair in _active)
        {
            tasks.Add(pair.Value.Task);
        }
        Task.WaitAll(tasks.ToArray(), timeout);
    }

    protected virtual void OnProgressChanged(DeliveryProgressEventArgs e)
    {
        var handler = ProgressChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    private void Run(Order order, TimeSpan stepDelay, CancellationToken token)
    {
        try
        {
            for (int step = 1; step <= ProgressSteps; step++)
            {
                if (token.WaitHandle.WaitOne(stepDelay))
                {
                    _service.InterruptDelivery(order);
                    return;
                }
                OnProgressChanged(new DeliveryProgressEventArgs(order, step * FullPercent / ProgressSteps));
            }
            _service.CompleteDelivery(order);
        }
        finally
        {
            ActiveDelivery? finished;
            _active.TryRemove(order.Id, out finished);
        }
    }
}

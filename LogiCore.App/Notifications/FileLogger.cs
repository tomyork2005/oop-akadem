using LogiCore.Domain.Events;
using LogiCore.Domain.Services;

namespace LogiCore.App.Notifications;

public class FileLogger
{
    private readonly string _path;
    private readonly object _sync = new object();

    public FileLogger(string path)
    {
        _path = path;
    }

    public void Subscribe(DeliveryService service)
    {
        service.OrderCreated += OnOrderCreated;
        service.OrderStatusChanged += OnOrderStatusChanged;
        service.VehicleOverloadAttempt += OnVehicleOverloadAttempt;
        service.DeliveryCompleted += OnDeliveryCompleted;
    }

    private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        Append($"CREATED   {e.Order.Number} {e.Order.Customer.Name}, {e.Order.Route}");
    }

    private void OnOrderStatusChanged(object sender, OrderStatusChangedEventArgs e)
    {
        Append($"STATUS    {e.Order.Number} {e.OldStatus} -> {e.NewStatus}");
    }

    private void OnVehicleOverloadAttempt(object? sender, VehicleOverloadAttemptEventArgs e)
    {
        Append($"OVERLOAD  {e.Order.Number} {e.Vehicle.RegistrationNumber}");
    }

    private void OnDeliveryCompleted(object? sender, DeliveryCompletedEventArgs e)
    {
        Append($"DELIVERED {e.Order.Number} выручка {e.Order.TotalCost:F2}");
    }

    private void Append(string line)
    {
        lock (_sync)
        {
            using (var writer = new StreamWriter(_path, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {line}");
            }
        }
    }
}

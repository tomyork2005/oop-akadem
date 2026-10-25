using LogiCore.Domain.Events;
using LogiCore.Domain.Services;
using LogiCore.Domain.Simulation;

namespace LogiCore.App.Notifications;

public class ConsoleNotifier
{
    public void Subscribe(DeliveryService service)
    {
        service.OrderCreated += OnOrderCreated;
        service.OrderStatusChanged += OnOrderStatusChanged;
        service.VehicleOverloadAttempt += OnVehicleOverloadAttempt;
        service.DeliveryCompleted += OnDeliveryCompleted;
    }

    public void Unsubscribe(DeliveryService service)
    {
        service.OrderCreated -= OnOrderCreated;
        service.OrderStatusChanged -= OnOrderStatusChanged;
        service.VehicleOverloadAttempt -= OnVehicleOverloadAttempt;
        service.DeliveryCompleted -= OnDeliveryCompleted;
    }

    public void Subscribe(DeliverySimulator simulator)
    {
        simulator.ProgressChanged += OnProgressChanged;
    }

    private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        Console.WriteLine($"    [событие] создан заказ {e.Order.Number}: {e.Order.Route}");
    }

    private void OnOrderStatusChanged(object sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"    [событие] заказ {e.Order.Number}: {e.OldStatus} -> {e.NewStatus} (поток {Environment.CurrentManagedThreadId})");
    }

    private void OnVehicleOverloadAttempt(object? sender, VehicleOverloadAttemptEventArgs e)
    {
        Console.WriteLine($"    [событие] попытка перегруза {e.Vehicle.RegistrationNumber}: {e.Order.TotalWeightKg} кг при лимите {e.Vehicle.MaxLoadKg} кг");
    }

    private void OnDeliveryCompleted(object? sender, DeliveryCompletedEventArgs e)
    {
        Console.WriteLine($"    [событие] заказ {e.Order.Number} доставлен, выручка {e.Order.TotalCost:N2} руб.");
    }

    private void OnProgressChanged(object sender, DeliveryProgressEventArgs e)
    {
        Console.WriteLine($"    [прогресс] заказ {e.Order.Number}: {e.Percent}% (поток {Environment.CurrentManagedThreadId})");
    }
}

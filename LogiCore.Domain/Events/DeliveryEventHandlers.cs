namespace LogiCore.Domain.Events;

public delegate void OrderStatusChangedHandler(object sender, OrderStatusChangedEventArgs e);

public delegate void DeliveryProgressHandler(object sender, DeliveryProgressEventArgs e);

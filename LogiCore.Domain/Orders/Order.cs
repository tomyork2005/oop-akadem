using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Routing;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Orders;

public class Order : IEntity
{
    public const int NumberLength = 8;

    private readonly List<Cargo> _cargo;

    public Order(Guid id, Customer customer, IEnumerable<Cargo> cargo, Route route, string tariffName, ExtraServices services)
    {
        _cargo = new List<Cargo>(cargo);
        if (_cargo.Count == 0)
        {
            throw new CargoValidationException("в заказе должен быть хотя бы один груз");
        }

        Id = id;
        Customer = customer;
        Route = route;
        TariffName = tariffName;
        Services = services;
        Status = OrderStatus.Created;
    }

    public Guid Id { get; }
    public Customer Customer { get; }
    public Route Route { get; }
    public string TariffName { get; }
    public ExtraServices Services { get; }

    public OrderStatus Status { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public decimal TotalCost { get; private set; }

    public IReadOnlyCollection<Cargo> CargoItems
    {
        get { return _cargo.AsReadOnly(); }
    }

    public string Number
    {
        get { return Id.ToString("N").Substring(0, NumberLength).ToUpperInvariant(); }
    }

    public decimal TotalWeightKg
    {
        get { return CargoTotals.Weight(_cargo); }
    }

    public decimal TotalVolumeM3
    {
        get { return CargoTotals.Volume(_cargo); }
    }

    public bool HasService(ExtraServices service)
    {
        return (Services & service) == service;
    }

    public void Assign(Vehicle vehicle, decimal totalCost)
    {
        MoveTo(OrderStatus.Assigned);
        Vehicle = vehicle;
        TotalCost = totalCost;
    }

    public void StartDelivery()
    {
        MoveTo(OrderStatus.InTransit);
    }

    public void Complete()
    {
        MoveTo(OrderStatus.Delivered);
    }

    public void Cancel()
    {
        MoveTo(OrderStatus.Cancelled);
    }

    public void Interrupt()
    {
        MoveTo(OrderStatus.Interrupted);
    }

    internal void Restore(OrderStatus status, Vehicle? vehicle, decimal totalCost)
    {
        Status = status;
        Vehicle = vehicle;
        TotalCost = totalCost;
    }

    public override string ToString()
    {
        var vehicle = Vehicle == null ? "ТС не назначено" : Vehicle.RegistrationNumber;
        return $"Заказ {Number} [{Status}] {Customer.Name}: {Route}, {vehicle}, {TotalCost:N2} руб.";
    }

    private void MoveTo(OrderStatus next)
    {
        if (!IsTransitionAllowed(Status, next))
        {
            throw new InvalidOrderStateException(Id, Status, next);
        }
        Status = next;
    }

    private static bool IsTransitionAllowed(OrderStatus from, OrderStatus to)
    {
        switch (from)
        {
            case OrderStatus.Created:
                return to == OrderStatus.Assigned || to == OrderStatus.Cancelled;
            case OrderStatus.Assigned:
                return to == OrderStatus.InTransit || to == OrderStatus.Cancelled;
            case OrderStatus.InTransit:
                return to == OrderStatus.Delivered || to == OrderStatus.Interrupted;
            default:
                return false;
        }
    }
}

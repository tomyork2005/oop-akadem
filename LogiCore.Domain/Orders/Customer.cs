using LogiCore.Domain.Common;

namespace LogiCore.Domain.Orders;

public class Customer : IEntity
{
    private readonly List<Order> _orders = new List<Order>();

    public Customer(Guid id, string name, string contact)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("имя клиента не может быть пустым", nameof(name));
        }
        Id = id;
        Name = name;
        Contact = contact;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Contact { get; }

    public IReadOnlyCollection<Order> Orders
    {
        get { return _orders.AsReadOnly(); }
    }

    internal void AddOrder(Order order)
    {
        _orders.Add(order);
    }

    public override string ToString()
    {
        return $"{Name} ({Contact})";
    }
}

namespace LogiCore.Domain.Pricing;

public class BaseDeliveryCost : IDeliveryCost
{
    private readonly string _description;

    public BaseDeliveryCost(decimal amount, string description)
    {
        Total = amount;
        _description = description;
    }

    public decimal Total { get; }

    public string Describe()
    {
        return $"{_description}: {Total:N2}";
    }
}

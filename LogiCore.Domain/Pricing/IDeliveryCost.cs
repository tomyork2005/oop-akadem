namespace LogiCore.Domain.Pricing;

public interface IDeliveryCost
{
    decimal Total { get; }

    string Describe();
}

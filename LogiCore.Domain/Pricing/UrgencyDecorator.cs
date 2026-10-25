namespace LogiCore.Domain.Pricing;

public class UrgencyDecorator : DeliveryCostDecorator
{
    private readonly decimal _multiplier;

    public UrgencyDecorator(IDeliveryCost inner, decimal multiplier) : base(inner)
    {
        _multiplier = multiplier;
    }

    public override decimal Total
    {
        get { return Inner.Total * _multiplier; }
    }

    protected override string DescribeStep()
    {
        return $"  × срочность {_multiplier}: {Inner.Total:N2} → {Total:N2}";
    }
}

namespace LogiCore.Domain.Pricing;

public abstract class DeliveryCostDecorator : IDeliveryCost
{
    protected DeliveryCostDecorator(IDeliveryCost inner)
    {
        Inner = inner;
    }

    protected IDeliveryCost Inner { get; }

    public abstract decimal Total { get; }

    public string Describe()
    {
        return Inner.Describe() + Environment.NewLine + DescribeStep();
    }

    protected abstract string DescribeStep();
}

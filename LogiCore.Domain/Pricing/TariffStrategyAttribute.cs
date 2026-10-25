namespace LogiCore.Domain.Pricing;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TariffStrategyAttribute : Attribute
{
    public TariffStrategyAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

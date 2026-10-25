using LogiCore.Domain.Cargos;

namespace LogiCore.Domain.Pricing;

public class InsuranceDecorator : DeliveryCostDecorator
{
    public const decimal PercentBase = 100m;

    private readonly IReadOnlyCollection<Cargo> _cargo;
    private readonly decimal _percent;

    public InsuranceDecorator(IDeliveryCost inner, IReadOnlyCollection<Cargo> cargo, decimal percent) : base(inner)
    {
        _cargo = cargo;
        _percent = percent;
    }

    public decimal Premium
    {
        get
        {
            decimal insured = 0;
            foreach (var item in _cargo)
            {
                var value = item.DeclaredValue;
                var insurable = item as IInsurable;
                if (insurable != null)
                {
                    value = value * insurable.RiskFactor;
                }
                insured += value;
            }
            return insured * _percent / PercentBase;
        }
    }

    public override decimal Total
    {
        get { return Inner.Total + Premium; }
    }

    protected override string DescribeStep()
    {
        return $"  + страховка {_percent}%: +{Premium:N2} → {Total:N2}";
    }
}

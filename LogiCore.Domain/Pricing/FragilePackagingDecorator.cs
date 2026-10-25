using LogiCore.Domain.Cargos;

namespace LogiCore.Domain.Pricing;

public class FragilePackagingDecorator : DeliveryCostDecorator
{
    private readonly IReadOnlyCollection<Cargo> _cargo;
    private readonly decimal _feePerItem;

    public FragilePackagingDecorator(IDeliveryCost inner, IReadOnlyCollection<Cargo> cargo, decimal feePerItem) : base(inner)
    {
        _cargo = cargo;
        _feePerItem = feePerItem;
    }

    public decimal Fee
    {
        get
        {
            decimal fee = 0;
            foreach (var item in _cargo)
            {
                if (item is FragileCargo)
                {
                    fee += _feePerItem;
                }
            }
            return fee;
        }
    }

    public override decimal Total
    {
        get { return Inner.Total + Fee; }
    }

    protected override string DescribeStep()
    {
        return $"  + упаковка хрупкого: +{Fee:N2} → {Total:N2}";
    }
}

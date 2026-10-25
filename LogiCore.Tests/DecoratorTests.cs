using LogiCore.Domain.Cargos;
using LogiCore.Domain.Pricing;

namespace LogiCore.Tests;

public class DecoratorTests
{
    private readonly Cargo[] _items =
    {
        new StandardCargo(Guid.NewGuid(), "Коробки", 10m, 1m, 10000m),
        new FragileCargo(Guid.NewGuid(), "Стекло", 5m, 1m, 10000m, 0.5m)
    };

    [Fact]
    public void TwoLayers_InsuranceThenUrgency()
    {
        IDeliveryCost cost = new BaseDeliveryCost(1000m, "база");
        cost = new InsuranceDecorator(cost, _items, 1m);
        cost = new UrgencyDecorator(cost, 1.5m);

        Assert.Equal((1000m + 250m) * 1.5m, cost.Total);
    }

    [Fact]
    public void ThreeLayers_InsurancePackagingUrgency()
    {
        IDeliveryCost cost = new BaseDeliveryCost(1000m, "база");
        cost = new InsuranceDecorator(cost, _items, 1m);
        cost = new FragilePackagingDecorator(cost, _items, 300m);
        cost = new UrgencyDecorator(cost, 1.5m);

        Assert.Equal((1000m + 250m + 300m) * 1.5m, cost.Total);
    }

    [Fact]
    public void OrderOfLayers_ChangesTotal()
    {
        var start = new BaseDeliveryCost(1000m, "база");
        var insuranceFirst = new UrgencyDecorator(new InsuranceDecorator(start, _items, 1m), 1.5m);
        var urgencyFirst = new InsuranceDecorator(new UrgencyDecorator(start, 1.5m), _items, 1m);

        Assert.Equal(1875m, insuranceFirst.Total);
        Assert.Equal(1750m, urgencyFirst.Total);
    }
}

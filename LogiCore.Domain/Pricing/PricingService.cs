using LogiCore.Domain.Orders;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Domain.Pricing;

public class PricingService
{
    private readonly TariffRegistry _tariffs;

    public PricingService(TariffRegistry tariffs)
    {
        _tariffs = tariffs;
    }

    public IDeliveryCost Calculate(Order order, Vehicle vehicle)
    {
        var tariff = _tariffs.Find(order.TariffName);
        if (tariff == null)
        {
            throw new ArgumentException($"тариф «{order.TariffName}» не найден");
        }

        var settings = TariffSettings.Instance;
        var vehicleCost = vehicle.CalculateDeliveryCost(order.Route, order.CargoItems);
        var tariffCost = tariff.Calculate(vehicleCost, order.Route, order.CargoItems);

        IDeliveryCost cost = new BaseDeliveryCost(tariffCost,
            $"  перевозка {vehicle.RegistrationNumber} {vehicleCost:N2} по тарифу {tariff.Name}");
        if (order.HasService(ExtraServices.Insurance))
        {
            cost = new InsuranceDecorator(cost, order.CargoItems, settings.InsurancePercent);
        }
        if (order.HasService(ExtraServices.FragilePackaging))
        {
            cost = new FragilePackagingDecorator(cost, order.CargoItems, settings.FragilePackagingFee);
        }
        if (order.HasService(ExtraServices.Urgent))
        {
            cost = new UrgencyDecorator(cost, settings.UrgencyMultiplier);
        }
        return cost;
    }
}

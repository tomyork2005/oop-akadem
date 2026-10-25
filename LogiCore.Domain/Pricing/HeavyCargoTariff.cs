using LogiCore.Domain.Cargos;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Pricing;

public class HeavyCargoTariff : ITariffStrategy
{
    public const string TariffName = "HeavyCargo";

    public string Name
    {
        get { return TariffName; }
    }

    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
    {
        var settings = TariffSettings.Instance;
        var weight = CargoTotals.Weight(cargo);
        if (weight <= settings.HeavyCargoThresholdKg)
        {
            return baseCost;
        }
        return baseCost + (weight - settings.HeavyCargoThresholdKg) * settings.HeavyCargoSurchargePerKg;
    }
}

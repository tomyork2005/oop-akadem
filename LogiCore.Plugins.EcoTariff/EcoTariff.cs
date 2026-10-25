using LogiCore.Domain.Cargos;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Routing;

namespace LogiCore.Plugins.EcoTariff;

[TariffStrategy(TariffName)]
public class EcoTariff : ITariffStrategy
{
    public const string TariffName = "Eco";
    public const decimal LongRouteKm = 1000m;
    public const decimal LongRouteDiscount = 0.9m;

    public string Name
    {
        get { return TariffName; }
    }

    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
    {
        if (route.DistanceKm >= LongRouteKm)
        {
            return baseCost * LongRouteDiscount;
        }
        return baseCost;
    }
}

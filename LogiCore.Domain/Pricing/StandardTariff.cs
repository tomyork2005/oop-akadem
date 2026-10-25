using LogiCore.Domain.Cargos;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Pricing;

public class StandardTariff : ITariffStrategy
{
    public const string TariffName = "Standard";

    public string Name
    {
        get { return TariffName; }
    }

    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
    {
        return baseCost;
    }
}

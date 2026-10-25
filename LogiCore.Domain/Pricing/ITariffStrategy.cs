using LogiCore.Domain.Cargos;
using LogiCore.Domain.Routing;

namespace LogiCore.Domain.Pricing;

public interface ITariffStrategy
{
    string Name { get; }

    decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo);
}

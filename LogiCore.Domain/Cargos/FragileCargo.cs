using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Cargos;

public class FragileCargo : Cargo, IInsurable
{
    public const decimal DefaultRiskFactor = 0.3m;
    public const decimal MaxRiskFactor = 1m;
    public const decimal InsuranceBaseRisk = 1m;

    public FragileCargo(Guid id, string description, decimal weightKg, decimal volumeM3, decimal declaredValue, decimal riskFactor)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        if (riskFactor <= 0 || riskFactor > MaxRiskFactor)
        {
            throw new CargoValidationException($"коэффициент риска должен быть в диапазоне (0; {MaxRiskFactor}]");
        }
        RiskFactor = riskFactor;
    }

    public decimal RiskFactor { get; }

    public override string Category
    {
        get { return "Хрупкий"; }
    }

    decimal IInsurable.RiskFactor
    {
        get { return InsuranceBaseRisk + RiskFactor; }
    }
}

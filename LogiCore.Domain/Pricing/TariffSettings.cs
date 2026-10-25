namespace LogiCore.Domain.Pricing;

public sealed class TariffSettings
{
    public const int MoneyPrecision = 2;

    private static readonly Lazy<TariffSettings> LazyInstance = new Lazy<TariffSettings>(() => new TariffSettings());

    private TariffSettings()
    {
        ExpressMultiplier = 1.5m;
        HeavyCargoThresholdKg = 5000m;
        HeavyCargoSurchargePerKg = 1.2m;
        InsurancePercent = 0.3m;
        UrgencyMultiplier = 1.3m;
        FragilePackagingFee = 1500m;
    }

    public static TariffSettings Instance
    {
        get { return LazyInstance.Value; }
    }

    public decimal ExpressMultiplier { get; }
    public decimal HeavyCargoThresholdKg { get; }
    public decimal HeavyCargoSurchargePerKg { get; }
    public decimal InsurancePercent { get; }
    public decimal UrgencyMultiplier { get; }
    public decimal FragilePackagingFee { get; }
}

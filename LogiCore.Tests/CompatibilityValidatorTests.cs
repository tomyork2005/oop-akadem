using LogiCore.Domain.Cargos;
using LogiCore.Domain.Validation;

namespace LogiCore.Tests;

public class CompatibilityValidatorTests
{
    private readonly CargoCompatibilityValidator _validator = new CargoCompatibilityValidator();

    [Fact]
    public void Rule1_DangerousTogetherWithPerishable_IsInvalid()
    {
        var result = _validator.Validate(null, new Cargo[] { TestData.Dangerous(), TestData.Perishable() }, DateTime.Now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Rule2_PerishableInPlainTruck_IsInvalid()
    {
        var result = _validator.Validate(TestData.Truck(), new Cargo[] { TestData.Perishable() }, DateTime.Now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Rule2_PerishableInReeferWithRightTemperature_IsValid()
    {
        var result = _validator.Validate(TestData.Reefer(), new Cargo[] { TestData.Perishable(-18m) }, DateTime.Now);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Rule3_TotalWeightAboveLimit_IsInvalid()
    {
        var result = _validator.Validate(TestData.Truck(), new Cargo[] { TestData.Standard(6000m), TestData.Standard(6000m) }, DateTime.Now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Rule4_ExpiredCargo_IsInvalid()
    {
        var result = _validator.Validate(TestData.Reefer(), new Cargo[] { TestData.Perishable(4m, -1) }, DateTime.Now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Contravariance_CargoValidatorWorksForPerishable()
    {
        IValidator<Cargo> cargoValidator = new CargoExpirationValidator(DateTime.Now);
        IValidator<PerishableCargo> perishableValidator = cargoValidator;

        Assert.False(perishableValidator.Validate(TestData.Perishable(4m, -2)).IsValid);
    }
}

using LogiCore.Domain.Cargos;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Vehicles;

namespace LogiCore.Tests;

public class VehicleCostTests
{
    [Fact]
    public void Truck_Cost_IsDistanceTimesRateTimesTollFactor()
    {
        var cost = TestData.Truck().CalculateDeliveryCost(TestData.EquatorRoute(), new Cargo[] { TestData.Standard() });

        Assert.Equal(TestData.EquatorDegreeKm * 10m * 1.2m, cost);
    }

    [Fact]
    public void Truck_ZeroDistance_CostIsZero()
    {
        var cost = TestData.Truck().CalculateDeliveryCost(TestData.ZeroRoute(), new Cargo[] { TestData.Standard() });

        Assert.Equal(0m, cost);
    }

    [Fact]
    public void RefrigeratorTruck_Cost_IsTruckCostPlusCooling()
    {
        var cost = TestData.Reefer().CalculateDeliveryCost(TestData.EquatorRoute(), new Cargo[] { TestData.Perishable() });

        Assert.Equal(TestData.EquatorDegreeKm * 12m * 1.1m + TestData.EquatorDegreeKm * 3m, cost);
    }

    [Fact]
    public void RefrigeratorTruck_CanCarry_ChecksTemperature()
    {
        var reefer = TestData.Reefer();

        Assert.True(reefer.CanCarry(TestData.Perishable(4m)));
        Assert.False(reefer.CanCarry(TestData.Perishable(-30m)));
    }

    [Fact]
    public void CargoPlane_Cost_AddsSurchargeForEveryKg()
    {
        var cargo = new Cargo[] { TestData.Standard(100m), TestData.Standard(200m) };

        var cost = TestData.Plane().CalculateDeliveryCost(TestData.EquatorRoute(), cargo);

        Assert.Equal(TestData.EquatorDegreeKm * 100m + 300m * 2m, cost);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(7, false)]
    [InlineData(3, true)]
    public void CargoPlane_CanCarry_ForbidsSomeHazardClasses(int hazardClass, bool expected)
    {
        Assert.Equal(expected, TestData.Plane().CanCarry(TestData.Dangerous(hazardClass)));
    }

    [Fact]
    public void CargoShip_Cost_AddsSurchargeForOversized()
    {
        var cargo = new Cargo[] { TestData.Oversized(), TestData.Oversized(), TestData.Standard() };

        var cost = TestData.Ship().CalculateDeliveryCost(TestData.EquatorRoute(), cargo);

        Assert.Equal(TestData.EquatorDegreeKm * 5m + 2 * 1000m, cost);
    }

    [Fact]
    public void DroneCourier_ZeroDistance_CostIsLaunchFee()
    {
        var cost = TestData.Drone().CalculateDeliveryCost(TestData.ZeroRoute(), new Cargo[] { TestData.Standard(1m, 0.01m) });

        Assert.Equal(100m, cost);
    }

    [Theory]
    [InlineData(10.0, true)]
    [InlineData(10.01, false)]
    public void DroneCourier_CanCarry_LimitWeight(double weight, bool expected)
    {
        Assert.Equal(expected, TestData.Drone().CanCarry(TestData.Standard((decimal)weight, 0.01m)));
    }

    [Fact]
    public void DroneCourier_OutOfRange_ThrowsRouteNotFound()
    {
        Assert.Throws<RouteNotFoundException>(() =>
            TestData.Drone().CalculateDeliveryCost(TestData.EquatorRoute(), new Cargo[] { TestData.Standard(1m, 0.01m) }));
    }

    [Theory]
    [InlineData("trk-001")]
    [InlineData("TRK001")]
    public void Vehicle_InvalidRegistrationNumber_Throws(string number)
    {
        Assert.Throws<ArgumentException>(() => new Truck(Guid.NewGuid(), number, 1000m, 10m, 60m, 10m, 1m));
    }
}

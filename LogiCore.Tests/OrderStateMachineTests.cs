using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Orders;

namespace LogiCore.Tests;

public class OrderStateMachineTests
{
    [Fact]
    public void HappyPath_CreatedAssignedInTransitDelivered()
    {
        var order = TestData.Order(TestData.Standard());

        order.Assign(TestData.Truck(), 100m);
        order.StartDelivery();
        order.Complete();

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Cancel_FromCreated_IsAllowed()
    {
        var order = TestData.Order(TestData.Standard());

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_FromAssigned_IsAllowed()
    {
        var order = TestData.Order(TestData.Standard());
        order.Assign(TestData.Truck(), 100m);

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void StartDelivery_FromCreated_Throws()
    {
        var order = TestData.Order(TestData.Standard());

        Assert.Throws<InvalidOrderStateException>(() => order.StartDelivery());
        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public void Complete_FromAssigned_Throws()
    {
        var order = TestData.Order(TestData.Standard());
        order.Assign(TestData.Truck(), 100m);

        Assert.Throws<InvalidOrderStateException>(() => order.Complete());
    }

    [Fact]
    public void Cancel_FromInTransit_Throws()
    {
        var order = TestData.Order(TestData.Standard());
        order.Assign(TestData.Truck(), 100m);
        order.StartDelivery();

        Assert.Throws<InvalidOrderStateException>(() => order.Cancel());
    }
}

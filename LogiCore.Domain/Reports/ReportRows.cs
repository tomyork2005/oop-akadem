using LogiCore.Domain.Orders;

namespace LogiCore.Domain.Reports;

public class VehicleRevenueRow
{
    public VehicleRevenueRow(string registrationNumber, string vehicleType, decimal revenue)
    {
        RegistrationNumber = registrationNumber;
        VehicleType = vehicleType;
        Revenue = revenue;
    }

    public string RegistrationNumber { get; }
    public string VehicleType { get; }
    public decimal Revenue { get; }
}

public class StatusSummaryRow
{
    public StatusSummaryRow(OrderStatus status, int orderCount, decimal totalCost)
    {
        Status = status;
        OrderCount = orderCount;
        TotalCost = totalCost;
    }

    public OrderStatus Status { get; }
    public int OrderCount { get; }
    public decimal TotalCost { get; }
}

public class VehicleLoadRow
{
    public VehicleLoadRow(string vehicleType, decimal averageLoadPercent)
    {
        VehicleType = vehicleType;
        AverageLoadPercent = averageLoadPercent;
    }

    public string VehicleType { get; }
    public decimal AverageLoadPercent { get; }
}

public class CustomerTotalRow
{
    public CustomerTotalRow(string customerName, decimal totalAmount)
    {
        CustomerName = customerName;
        TotalAmount = totalAmount;
    }

    public string CustomerName { get; }
    public decimal TotalAmount { get; }
}

public class CargoOrderCustomerRow
{
    public CargoOrderCustomerRow(string cargoDescription, string orderNumber, string customerName)
    {
        CargoDescription = cargoDescription;
        OrderNumber = orderNumber;
        CustomerName = customerName;
    }

    public string CargoDescription { get; }
    public string OrderNumber { get; }
    public string CustomerName { get; }
}

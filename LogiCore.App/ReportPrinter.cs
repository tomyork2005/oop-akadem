using System.Text;
using LogiCore.Domain.Common;
using LogiCore.Domain.Reports;

namespace LogiCore.App;

public class ReportPrinter
{
    public const int TopVehicleCount = 3;
    public const decimal CustomerThreshold = 50000m;

    private readonly ReportService _reports;

    public ReportPrinter(ReportService reports)
    {
        _reports = reports;
    }

    public string BuildAll()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Отчёт 1. Топ-3 ТС по выручке");
        sb.Append(_reports.TopVehiclesByRevenue(TopVehicleCount).ToReportTable(
            new[] { "Номер", "Тип", "Выручка" },
            r => new[] { r.RegistrationNumber, r.VehicleType, r.Revenue.ToString("N2") }));

        sb.AppendLine("Отчёт 2. Заказы по статусам");
        sb.Append(_reports.OrdersByStatus().ToReportTable(
            new[] { "Статус", "Кол-во", "Сумма" },
            r => new[] { r.Status.ToString(), r.OrderCount.ToString(), r.TotalCost.ToString("N2") }));

        sb.AppendLine("Отчёт 3. Средняя загрузка ТС по типам, %");
        sb.Append(_reports.AverageLoadByVehicleType().ToReportTable(
            new[] { "Тип ТС", "Загрузка, %" },
            r => new[] { r.VehicleType, r.AverageLoadPercent.ToString("F2") }));

        sb.AppendLine($"Отчёт 4. Клиенты с суммой заказов больше {CustomerThreshold:N0} руб.");
        sb.Append(_reports.CustomersAbove(CustomerThreshold).ToReportTable(
            new[] { "Клиент", "Сумма" },
            r => new[] { r.CustomerName, r.TotalAmount.ToString("N2") }));

        sb.AppendLine("Отчёт 5. Груз → заказ → клиент");
        sb.Append(_reports.CargoOrderCustomer().ToReportTable(
            new[] { "Груз", "Заказ", "Клиент" },
            r => new[] { r.CargoDescription, r.OrderNumber, r.CustomerName }));

        sb.AppendLine("Отчёт 6. Класс опасности → количество грузов");
        var hazard = _reports.CargoCountByHazardClass();
        var classes = new List<int>(hazard.Keys);
        classes.Sort();
        sb.Append(classes.ToReportTable(
            new[] { "Класс", "Грузов" },
            k => new[] { k.ToString(), hazard[k].ToString() }));

        return sb.ToString();
    }
}

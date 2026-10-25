using LogiCore.Domain.Cargos;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Routing;

namespace LogiCore.App;

public class ConsoleMenu
{
    public const int MenuStepSeconds = 3;

    private readonly AppServices _app;
    private readonly ReportPrinter _printer;
    private readonly ConsoleInput _input = new ConsoleInput();

    public ConsoleMenu(AppServices app)
    {
        _app = app;
        _printer = new ReportPrinter(app.Reports);
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Меню ===");
            Console.WriteLine("1. Создать заказ");
            Console.WriteLine("2. Показать отчёты");
            Console.WriteLine("3. Запустить доставку");
            Console.WriteLine("4. Отменить доставку в пути");
            Console.WriteLine("5. Сохранить состояние");
            Console.WriteLine("6. Загрузить состояние");
            Console.WriteLine("0. Выход");

            var choice = _input.ReadLine("> ");
            if (choice == null || choice.Trim() == "0")
            {
                return;
            }
            try
            {
                Handle(choice.Trim());
            }
            catch (LogisticsException ex)
            {
                Console.WriteLine("ошибка: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ошибка: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("ошибка: " + ex.Message);
            }
        }
    }

    private void Handle(string choice)
    {
        switch (choice)
        {
            case "1":
                CreateOrder();
                break;
            case "2":
                Console.WriteLine(_printer.BuildAll());
                break;
            case "3":
                StartDelivery();
                break;
            case "4":
                CancelDelivery();
                break;
            case "5":
                _app.Storage.Save(_app.StatePath, _app.Data);
                Console.WriteLine("сохранено: " + _app.StatePath);
                break;
            case "6":
                LoadState();
                break;
            default:
                Console.WriteLine("нет такого пункта");
                break;
        }
    }

    private void CreateOrder()
    {
        var customers = new List<Customer>(_app.Data.Customers);
        var customerNames = new List<string>();
        foreach (var customer in customers)
        {
            customerNames.Add(customer.Name);
        }
        var customerIndex = Choose("клиент", customerNames);
        if (customerIndex == null)
        {
            return;
        }

        var cities = Cities.All();
        var cityNames = new List<string>();
        foreach (var city in cities)
        {
            cityNames.Add(city.Name);
        }
        var from = Choose("откуда", cityNames);
        if (from == null)
        {
            return;
        }
        var to = Choose("куда", cityNames);
        if (to == null)
        {
            return;
        }

        var cargo = ReadCargo();
        if (cargo == null)
        {
            return;
        }

        var tariffs = new List<string>(_app.Tariffs.Names);
        var tariffIndex = Choose("тариф", tariffs);
        if (tariffIndex == null)
        {
            return;
        }

        var services = ExtraServices.None;
        if (_input.AskYes("страховка?"))
        {
            services = services | ExtraServices.Insurance;
        }
        if (_input.AskYes("упаковка хрупкого?"))
        {
            services = services | ExtraServices.FragilePackaging;
        }
        if (_input.AskYes("срочная доставка?"))
        {
            services = services | ExtraServices.Urgent;
        }

        var route = new Route(new List<RoutePoint> { cities[from.Value], cities[to.Value] });
        var order = new Order(Guid.NewGuid(), customers[customerIndex.Value], new List<Cargo> { cargo }, route,
            tariffs[tariffIndex.Value], services);
        _app.Delivery.PlaceOrder(order);

        IDeliveryCost? cost;
        if (!_app.Delivery.TryAssignBestVehicle(order, out cost) || cost == null)
        {
            Console.WriteLine("свободного подходящего ТС нет, заказ ждёт");
            return;
        }
        Console.WriteLine("назначено: " + order.Vehicle);
        Console.WriteLine(cost.Describe());
        Console.WriteLine($"итого: {order.TotalCost:N2} руб.");
    }

    private Cargo? ReadCargo()
    {
        var kinds = new List<string>
        {
            CargoFactory.StandardKind,
            CargoFactory.PerishableKind,
            CargoFactory.FragileKind,
            CargoFactory.DangerousKind,
            CargoFactory.OversizedKind
        };
        var kindIndex = Choose("тип груза", kinds);
        if (kindIndex == null)
        {
            return null;
        }
        var kind = kinds[kindIndex.Value];

        var description = _input.ReadLine("описание: ");
        var weight = _input.ReadDecimal("вес, кг: ");
        var volume = _input.ReadDecimal("объём, м³: ");
        var value = _input.ReadDecimal("стоимость, руб.: ");
        if (description == null || weight == null || volume == null || value == null)
        {
            return null;
        }

        var spec = new CargoSpec(kind, description, weight.Value, volume.Value, value.Value);
        if (kind == CargoFactory.PerishableKind)
        {
            spec.With(CargoFactory.ExpiresInDaysParameter, AskNumber("срок годности, дней: "));
            spec.With(CargoFactory.TemperatureParameter, AskNumber("температура, °C: "));
        }
        if (kind == CargoFactory.FragileKind)
        {
            spec.With(CargoFactory.RiskParameter, AskNumber("коэффициент риска (0..1]: "));
        }
        if (kind == CargoFactory.DangerousKind)
        {
            spec.With(CargoFactory.HazardClassParameter, AskNumber("класс опасности 1-9: "));
        }
        return _app.CargoFactory.Create(spec);
    }

    private void StartDelivery()
    {
        var ready = new List<Order>(_app.Data.Orders.FindAll(o => o.Status == OrderStatus.Assigned));
        if (ready.Count == 0)
        {
            Console.WriteLine("нет заказов в статусе Assigned");
            return;
        }
        var index = Choose("заказ", OrderNames(ready));
        if (index == null)
        {
            return;
        }
        _app.Simulator.Start(ready[index.Value], TimeSpan.FromSeconds(MenuStepSeconds));
        Console.WriteLine("доставка идёт в фоне");
    }

    private void CancelDelivery()
    {
        var active = new List<Order>(_app.Simulator.ActiveOrders);
        if (active.Count == 0)
        {
            Console.WriteLine("активных доставок нет");
            return;
        }
        var index = Choose("заказ", OrderNames(active));
        if (index == null)
        {
            return;
        }
        _app.Simulator.Cancel(active[index.Value].Id);
    }

    private void LoadState()
    {
        if (_app.Simulator.ActiveOrders.Count > 0)
        {
            Console.WriteLine("сначала дождитесь окончания доставок");
            return;
        }
        string error;
        if (_app.Storage.TryLoad(_app.StatePath, _app.Data, out error))
        {
            Console.WriteLine("состояние загружено");
        }
        else
        {
            Console.WriteLine("не загружено: " + error);
        }
    }

    private int? Choose(string title, IReadOnlyList<string> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {options[i]}");
        }
        var number = _input.ReadInt(title + ": ", 1, options.Count);
        if (number == null)
        {
            return null;
        }
        return number.Value - 1;
    }

    private decimal AskNumber(string prompt)
    {
        var number = _input.ReadDecimal(prompt);
        if (number == null)
        {
            return 0;
        }
        return number.Value;
    }

    private static List<string> OrderNames(List<Order> orders)
    {
        var names = new List<string>();
        foreach (var order in orders)
        {
            names.Add(order.ToString());
        }
        return names;
    }
}

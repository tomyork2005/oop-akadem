using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Orders;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Routing;
using LogiCore.Domain.Validation;
using LogiCore.Domain.Vehicles;

namespace LogiCore.App;

public class DemoScenario
{
    public const int SimulationStepMs = 200;
    public const int CancelAfterMs = 500;
    public const int WaitTimeoutMs = 10000;

    private readonly AppServices _app;
    private readonly ReportPrinter _printer;
    private readonly Dictionary<string, Vehicle> _vehicles = new Dictionary<string, Vehicle>();
    private readonly Dictionary<string, Cargo> _cargo = new Dictionary<string, Cargo>();
    private readonly Dictionary<string, Customer> _customers = new Dictionary<string, Customer>();
    private readonly Dictionary<string, Order> _orders = new Dictionary<string, Order>();

    public DemoScenario(AppServices app)
    {
        _app = app;
        _printer = new ReportPrinter(app.Reports);
    }

    public void Run()
    {
        ShowTariffs();
        CreateFleetAndCargo();
        ShowFlags();
        ShowVariance();
        PlaceOrders();
        ShowOverload();
        AssignVehicles();
        RunLifecycle();
        RunParallelDeliveries();
        ShowReports();
        SaveClearLoad();
    }

    private void ShowTariffs()
    {
        Section("Тарифы (E1: плагины из папки plugins)");
        foreach (var line in _app.PluginLog)
        {
            Console.WriteLine("  " + line);
        }
        Console.WriteLine("  тарифы: " + string.Join(", ", _app.Tariffs.Names));
    }

    private void CreateFleetAndCargo()
    {
        Section("1. Парк и грузы через фабрики");
        AddVehicle(new VehicleSpec(VehicleFactory.TruckKind, "TRK-101", 20000m, 82m, 70m, 30m)
            .With(VehicleFactory.TollParameter, 1.15m));
        AddVehicle(new VehicleSpec(VehicleFactory.TruckKind, "TRK-102", 10000m, 45m, 75m, 26m)
            .With(VehicleFactory.TollParameter, 1.1m));
        AddVehicle(new VehicleSpec(VehicleFactory.RefrigeratorKind, "REF-201", 12000m, 60m, 65m, 35m)
            .With(VehicleFactory.MinTemperatureParameter, -25m)
            .With(VehicleFactory.MaxTemperatureParameter, 5m)
            .With(VehicleFactory.CoolingRateParameter, 6m));
        AddVehicle(new VehicleSpec(VehicleFactory.RefrigeratorKind, "REF-202", 8000m, 40m, 65m, 33m)
            .With(VehicleFactory.MinTemperatureParameter, 0m)
            .With(VehicleFactory.MaxTemperatureParameter, 10m));
        AddVehicle(new VehicleSpec(VehicleFactory.PlaneKind, "AIR-301", 40000m, 300m, 750m, 120m)
            .With(VehicleFactory.PerKgSurchargeParameter, 2m));
        AddVehicle(new VehicleSpec(VehicleFactory.ShipKind, "SEA-401", 500000m, 3000m, 30m, 12m)
            .With(VehicleFactory.OversizedSurchargeParameter, 3000m));
        AddVehicle(new VehicleSpec(VehicleFactory.DroneKind, "DRN-501", 20m, 0.2m, 60m, 15m)
            .With(VehicleFactory.MaxRangeParameter, 40m)
            .With(VehicleFactory.LaunchFeeParameter, 150m));
        AddVehicle(new VehicleSpec(VehicleFactory.DroneKind, "DRN-502", 10m, 0.1m, 70m, 12m)
            .With(VehicleFactory.MaxRangeParameter, 25m)
            .With(VehicleFactory.LaunchFeeParameter, 120m));

        AddCargo("appliances", new CargoSpec(CargoFactory.StandardKind, "Бытовая техника", 3200m, 18m, 900000m));
        AddCargo("furniture", new CargoSpec(CargoFactory.StandardKind, "Офисная мебель", 1800m, 22m, 350000m));
        AddCargo("documents", new CargoSpec(CargoFactory.StandardKind, "Пакет документов", 2m, 0.01m, 5000m));
        AddCargo("tool", new CargoSpec(CargoFactory.StandardKind, "Промышленный станок", 9000m, 25m, 4000000m));
        AddCargo("milk", new CargoSpec(CargoFactory.PerishableKind, "Молочная продукция", 2500m, 12m, 400000m)
            .With(CargoFactory.ExpiresInDaysParameter, 5m)
            .With(CargoFactory.TemperatureParameter, 4m));
        AddCargo("fish", new CargoSpec(CargoFactory.PerishableKind, "Замороженная рыба", 4000m, 15m, 1200000m)
            .With(CargoFactory.ExpiresInDaysParameter, 30m)
            .With(CargoFactory.TemperatureParameter, -18m));
        AddCargo("flowers", new CargoSpec(CargoFactory.PerishableKind, "Свежие цветы", 300m, 5m, 150000m)
            .With(CargoFactory.ExpiresInDaysParameter, 3m)
            .With(CargoFactory.TemperatureParameter, 6m));
        AddCargo("crystal", new CargoSpec(CargoFactory.FragileKind, "Хрустальная посуда", 600m, 4m, 800000m)
            .With(CargoFactory.RiskParameter, 0.4m));
        AddCargo("medical", new CargoSpec(CargoFactory.FragileKind, "Медицинское оборудование", 900m, 6m, 2500000m)
            .With(CargoFactory.RiskParameter, 0.25m));
        AddCargo("paint", new CargoSpec(CargoFactory.DangerousKind, "Лакокрасочные материалы", 6500m, 10m, 600000m)
            .With(CargoFactory.HazardClassParameter, 3m));
        AddCargo("batteries", new CargoSpec(CargoFactory.DangerousKind, "Литиевые аккумуляторы", 1200m, 3m, 700000m)
            .With(CargoFactory.HazardClassParameter, 9m));
        AddCargo("turbine", new CargoSpec(CargoFactory.OversizedKind, "Турбина ГЭС", 18000m, 90m, 15000000m));
        AddCargo("bridge", new CargoSpec(CargoFactory.OversizedKind, "Секция моста", 25000m, 120m, 9000000m));

        AddCustomer("techno", "ООО «ТехноМир»", "logistics@technomir.ru");
        AddCustomer("fish", "АО «СеверФиш»", "+7 921 555-12-34");
        AddCustomer("petrov", "ИП Петров", "petrov@mail.ru");
        AddCustomer("bridge", "ПАО «МостСтрой»", "info@moststroy.ru");
        AddCustomer("chem", "ООО «ХимПром»", "zakaz@himprom.ru");
    }

    private void ShowFlags()
    {
        Section("T10. [Flags] TransportConditions");
        var need = TransportConditions.Refrigerated | TransportConditions.Sealed;
        Console.WriteLine($"  need = Refrigerated | Sealed = {need}");
        foreach (var vehicle in _vehicles.Values)
        {
            Console.WriteLine($"  {vehicle.RegistrationNumber}: {vehicle.Conditions}, (c & need) == need: {(vehicle.Conditions & need) == need}");
        }
        Console.WriteLine($"  need & ~Sealed = {need & ~TransportConditions.Sealed}");
        Console.WriteLine($"  need ^ LongRange = {need ^ TransportConditions.LongRange}");
    }

    private void ShowVariance()
    {
        Section("T3. Ковариантность и контравариантность");
        var perishables = new Repository<PerishableCargo>();
        perishables.Add((PerishableCargo)_cargo["milk"]);
        perishables.Add((PerishableCargo)_cargo["fish"]);
        IReadOnlyRepository<Cargo> cargoView = perishables;
        Console.WriteLine($"  IReadOnlyRepository<Cargo> cargoView = Repository<PerishableCargo>: {cargoView.Count} шт.");

        IValidator<Cargo> cargoValidator = new CargoExpirationValidator(DateTime.Now);
        IValidator<PerishableCargo> perishableValidator = cargoValidator;
        foreach (var item in perishables)
        {
            Console.WriteLine($"  IValidator<PerishableCargo> perishableValidator = cargoValidator: {item.Description} — {perishableValidator.Validate(item)}");
        }
    }

    private void PlaceOrders()
    {
        Section("2. Оформление заказов");
        Place("O1", "techno", Path("Москва", "Санкт-Петербург"), StandardTariff.TariffName,
            ExtraServices.Insurance, "appliances", "furniture");
        Place("O2", "fish", Path("Санкт-Петербург", "Москва", "Казань"), ExpressTariff.TariffName,
            ExtraServices.Urgent, "milk", "fish");
        Place("O3", "petrov", Path("Москва", "Екатеринбург"), ExpressTariff.TariffName,
            ExtraServices.Insurance | ExtraServices.FragilePackaging | ExtraServices.Urgent, "crystal", "medical");
        Place("O4", "bridge", Path("Санкт-Петербург", "Калининград"), HeavyCargoTariff.TariffName,
            ExtraServices.None, "turbine", "bridge");
        Place("O5", "techno", Path("Москва", "Химки"), StandardTariff.TariffName,
            ExtraServices.Urgent, "documents");

        Console.WriteLine("  невалидный заказ: опасный груз + скоропортящийся");
        try
        {
            Place("bad", "chem", Path("Москва", "Краснодар"), StandardTariff.TariffName,
                ExtraServices.None, "batteries", "flowers");
        }
        catch (IncompatibleCargoException ex) when (ex.Violations.Count > 0)
        {
            Console.WriteLine("  перехвачено IncompatibleCargoException: " + ex.Message);
        }
    }

    private void ShowOverload()
    {
        Section("3. Попытка перегруза");
        Place("O6", "techno", Path("Москва", "Зеленоград"), StandardTariff.TariffName, ExtraServices.None, "tool");
        try
        {
            _app.Delivery.AssignVehicle(_orders["O6"], _vehicles["DRN-501"]);
        }
        catch (VehicleOverloadException ex) when (ex.RequestedWeightKg > ex.MaxWeightKg)
        {
            Console.WriteLine("  перехвачено VehicleOverloadException: " + ex.Message);
        }
        _app.Delivery.CancelOrder(_orders["O6"]);
    }

    private void AssignVehicles()
    {
        Section("4. Подбор ТС и расчёт стоимости по шагам");
        foreach (var key in new[] { "O4", "O1", "O2", "O3", "O5" })
        {
            var order = _orders[key];
            IDeliveryCost? cost;
            if (!_app.Delivery.TryAssignBestVehicle(order, out cost) || cost == null)
            {
                Console.WriteLine($"  {key}: подходящего свободного ТС нет");
                continue;
            }
            Console.WriteLine($"  {key} → {order.Vehicle}");
            Console.WriteLine(cost.Describe());
            Console.WriteLine($"  итого: {order.TotalCost:N2} руб.");
        }
    }

    private void RunLifecycle()
    {
        Section("5. Жизненный цикл заказа O1 (события в консоль и в файл)");
        _app.Delivery.StartDelivery(_orders["O1"]);
        _app.Delivery.CompleteDelivery(_orders["O1"]);

        Console.WriteLine("  отписываем ConsoleNotifier и доставляем O5: в консоли тихо, журнал пишется");
        _app.Notifier.Unsubscribe(_app.Delivery);
        _app.Delivery.StartDelivery(_orders["O5"]);
        _app.Delivery.CompleteDelivery(_orders["O5"]);
        _app.Notifier.Subscribe(_app.Delivery);
        Console.WriteLine("  журнал: " + _app.LogPath);
    }

    private void RunParallelDeliveries()
    {
        Section("E2. Параллельные доставки O2, O3, O4, отмена O4 в пути");
        var keys = new[] { "O2", "O3", "O4" };
        var tasks = new List<Task>();
        foreach (var key in keys)
        {
            tasks.Add(_app.Simulator.Start(_orders[key], TimeSpan.FromMilliseconds(SimulationStepMs)));
        }
        Thread.Sleep(CancelAfterMs);
        _app.Simulator.Cancel(_orders["O4"].Id);
        Task.WaitAll(tasks.ToArray(), WaitTimeoutMs);
        foreach (var key in keys)
        {
            Console.WriteLine($"  {key}: {_orders[key].Status}");
        }
    }

    private void ShowReports()
    {
        Section("6. Отчёты");
        Console.WriteLine(_printer.BuildAll());
    }

    private void SaveClearLoad()
    {
        Section("7. Сохранение в JSON → очистка → загрузка");
        var before = _printer.BuildAll();
        _app.Storage.Save(_app.StatePath, _app.Data);
        Console.WriteLine("  сохранено: " + _app.StatePath);

        _app.Data.Clear();
        PrintCounts("  после очистки");

        string error;
        var loaded = _app.Storage.TryLoad(_app.StatePath, _app.Data, out error);
        PrintCounts($"  после загрузки ({loaded})");

        var after = _printer.BuildAll();
        Console.WriteLine(after);
        if (before == after)
        {
            Console.WriteLine("  отчёты до сохранения и после загрузки совпадают");
        }
        else
        {
            Console.WriteLine("  отчёты отличаются: " + error);
        }
    }

    private void AddVehicle(VehicleSpec spec)
    {
        var vehicle = _app.VehicleFactory.Create(spec);
        _app.Data.Vehicles.Add(vehicle);
        _vehicles[vehicle.RegistrationNumber] = vehicle;
        Console.WriteLine("  + " + vehicle);
    }

    private void AddCargo(string key, CargoSpec spec)
    {
        var cargo = _app.CargoFactory.Create(spec);
        _app.Data.CargoItems.Add(cargo);
        _cargo[key] = cargo;
        Console.WriteLine("  + " + cargo);
    }

    private void AddCustomer(string key, string name, string contact)
    {
        var customer = new Customer(Guid.NewGuid(), name, contact);
        _app.Data.Customers.Add(customer);
        _customers[key] = customer;
        Console.WriteLine("  + клиент " + customer);
    }

    private void Place(string key, string customerKey, Route route, string tariffName, ExtraServices services, params string[] cargoKeys)
    {
        var cargo = new List<Cargo>();
        foreach (var cargoKey in cargoKeys)
        {
            cargo.Add(_cargo[cargoKey]);
        }
        var order = new Order(Guid.NewGuid(), _customers[customerKey], cargo, route, tariffName, services);
        _app.Delivery.PlaceOrder(order);
        _orders[key] = order;
        Console.WriteLine($"  {key}: {order}");
    }

    private static Route Path(params string[] cities)
    {
        var points = new List<RoutePoint>();
        foreach (var city in cities)
        {
            points.Add(Cities.Get(city));
        }
        return new Route(points);
    }

    private void PrintCounts(string title)
    {
        Console.WriteLine($"{title}: ТС {_app.Data.Vehicles.Count}, грузов {_app.Data.CargoItems.Count}, " +
                          $"клиентов {_app.Data.Customers.Count}, заказов {_app.Data.Orders.Count}, " +
                          $"выручка {_app.Delivery.TotalRevenue:N2}");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine("### " + title);
    }
}

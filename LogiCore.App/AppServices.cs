using LogiCore.App.Notifications;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Persistence;
using LogiCore.Domain.Plugins;
using LogiCore.Domain.Pricing;
using LogiCore.Domain.Reports;
using LogiCore.Domain.Services;
using LogiCore.Domain.Simulation;

namespace LogiCore.App;

public class AppServices
{
    public const string DataFolderName = "data";
    public const string StateFileName = "state.json";
    public const string LogFileName = "delivery-log.txt";
    public const string PluginsFolderName = "plugins";
    public const int ShutdownWaitMs = 5000;

    public AppServices(string baseDirectory)
    {
        var dataDirectory = Path.Combine(baseDirectory, DataFolderName);
        Directory.CreateDirectory(dataDirectory);
        StatePath = Path.Combine(dataDirectory, StateFileName);
        LogPath = Path.Combine(dataDirectory, LogFileName);

        Data = new CompanyData();
        Tariffs = new TariffRegistry();
        Tariffs.Register(StandardTariff.TariffName, new StandardTariff());
        Tariffs.Register(ExpressTariff.TariffName, new ExpressTariff());
        Tariffs.Register(HeavyCargoTariff.TariffName, new HeavyCargoTariff());
        PluginLog = new TariffPluginLoader().LoadFromDirectory(Path.Combine(baseDirectory, PluginsFolderName), Tariffs);

        Delivery = new DeliveryService(Data, new PricingService(Tariffs));
        Reports = new ReportService(Data.Vehicles, Data.CargoItems, Data.Customers, Data.Orders, Data.Ledger);
        Storage = new JsonStateStorage();
        VehicleFactory = new VehicleFactory();
        CargoFactory = new CargoFactory();
        Simulator = new DeliverySimulator(Delivery);

        Notifier = new ConsoleNotifier();
        Logger = new FileLogger(LogPath);
        Notifier.Subscribe(Delivery);
        Notifier.Subscribe(Simulator);
        Logger.Subscribe(Delivery);
    }

    public string StatePath { get; }
    public string LogPath { get; }
    public CompanyData Data { get; }
    public TariffRegistry Tariffs { get; }
    public IReadOnlyCollection<string> PluginLog { get; }
    public DeliveryService Delivery { get; }
    public ReportService Reports { get; }
    public JsonStateStorage Storage { get; }
    public VehicleFactory VehicleFactory { get; }
    public CargoFactory CargoFactory { get; }
    public DeliverySimulator Simulator { get; }
    public ConsoleNotifier Notifier { get; }
    public FileLogger Logger { get; }

    public void Shutdown()
    {
        Simulator.CancelAll();
        Simulator.WaitAll(TimeSpan.FromMilliseconds(ShutdownWaitMs));
    }
}

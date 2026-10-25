using System.Text;

namespace LogiCore.App;

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var app = new AppServices(AppContext.BaseDirectory);
        try
        {
            new DemoScenario(app).Run();
            new ConsoleMenu(app).Run();
        }
        finally
        {
            app.Shutdown();
        }
    }
}

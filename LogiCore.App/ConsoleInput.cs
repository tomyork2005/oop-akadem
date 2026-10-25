using System.Globalization;

namespace LogiCore.App;

public class ConsoleInput
{
    public string? ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public decimal? ReadDecimal(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt);
            if (line == null)
            {
                return null;
            }
            decimal value;
            if (decimal.TryParse(line.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            Console.WriteLine("  нужно число");
        }
    }

    public int? ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            var line = ReadLine(prompt);
            if (line == null)
            {
                return null;
            }
            int value;
            if (int.TryParse(line.Trim(), out value) && value >= min && value <= max)
            {
                return value;
            }
            Console.WriteLine($"  нужно число от {min} до {max}");
        }
    }

    public bool AskYes(string prompt)
    {
        var line = ReadLine(prompt + " (y/n): ");
        return line != null && line.Trim().ToLowerInvariant() == "y";
    }
}

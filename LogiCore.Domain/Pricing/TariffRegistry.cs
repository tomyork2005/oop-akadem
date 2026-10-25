namespace LogiCore.Domain.Pricing;

public class TariffRegistry
{
    private readonly Dictionary<string, ITariffStrategy> _tariffs = new Dictionary<string, ITariffStrategy>();

    public IReadOnlyCollection<string> Names
    {
        get { return new List<string>(_tariffs.Keys); }
    }

    public bool Register(string name, ITariffStrategy tariff)
    {
        if (_tariffs.ContainsKey(name))
        {
            return false;
        }
        _tariffs.Add(name, tariff);
        return true;
    }

    public ITariffStrategy? Find(string name)
    {
        ITariffStrategy? tariff;
        if (_tariffs.TryGetValue(name, out tariff))
        {
            return tariff;
        }
        return null;
    }
}

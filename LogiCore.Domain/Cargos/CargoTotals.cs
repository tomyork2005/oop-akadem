namespace LogiCore.Domain.Cargos;

public static class CargoTotals
{
    public static decimal Weight(IEnumerable<Cargo> cargo)
    {
        decimal sum = 0;
        foreach (var item in cargo)
        {
            sum += item.WeightKg;
        }
        return sum;
    }

    public static decimal Volume(IEnumerable<Cargo> cargo)
    {
        decimal sum = 0;
        foreach (var item in cargo)
        {
            sum += item.VolumeM3;
        }
        return sum;
    }
}

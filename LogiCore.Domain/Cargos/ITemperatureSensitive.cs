namespace LogiCore.Domain.Cargos;

public interface ITemperatureSensitive
{
    decimal RequiredTemperatureC { get; }
}

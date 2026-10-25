using LogiCore.Domain.Cargos;

namespace LogiCore.Domain.Validation;

public class CargoExpirationValidator : IValidator<Cargo>
{
    private readonly DateTime _checkDate;

    public CargoExpirationValidator(DateTime checkDate)
    {
        _checkDate = checkDate;
    }

    public ValidationResult Validate(Cargo item)
    {
        var errors = new List<string>();
        if (item.IsExpired(_checkDate))
        {
            errors.Add($"у груза «{item.Description}» истёк срок годности");
        }
        return new ValidationResult(errors);
    }
}

namespace LogiCore.Domain.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
}

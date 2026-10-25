namespace LogiCore.Domain.Validation;

public class ValidationResult
{
    private readonly List<string> _errors;

    public ValidationResult(IEnumerable<string> errors)
    {
        _errors = new List<string>(errors);
    }

    public bool IsValid
    {
        get { return _errors.Count == 0; }
    }

    public IReadOnlyCollection<string> Errors
    {
        get { return _errors.AsReadOnly(); }
    }

    public override string ToString()
    {
        if (IsValid)
        {
            return "ok";
        }
        return string.Join("; ", _errors);
    }
}

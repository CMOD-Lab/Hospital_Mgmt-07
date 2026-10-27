namespace ClinicManagement.Domain.Exceptions;

/// <summary>Thrown when a validation rule is violated.</summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

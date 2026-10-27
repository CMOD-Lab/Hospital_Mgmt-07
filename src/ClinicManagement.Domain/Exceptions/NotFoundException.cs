namespace ClinicManagement.Domain.Exceptions;

/// <summary>Thrown when a requested entity is not found.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}

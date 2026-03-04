namespace PayrollApi.Application.Common.Exceptions;

/// <summary>
/// Base application exception. Maps to HTTP 400 Bad Request by default.
/// </summary>
public class AppException : Exception
{
    public AppException(string message) : base(message) { }
}

/// <summary>Maps to HTTP 404 Not Found.</summary>
public class NotFoundException : AppException
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with identifier '{key}' was not found.") { }
}

/// <summary>Maps to HTTP 409 Conflict.</summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Maps to HTTP 403 Forbidden.</summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.") : base(message) { }
}

/// <summary>Maps to HTTP 422 Unprocessable Entity.</summary>
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = errors;
    }
}

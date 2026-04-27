using FluentResults;

namespace CursoLog.WebApi.Models.Errors;

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
    }
}

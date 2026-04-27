using FluentResults;

namespace CursoLog.WebApi.Models.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
    }
}

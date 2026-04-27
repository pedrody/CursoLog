using FluentResults;

namespace CursoLog.WebApi.Models.Errors;

public class ForbidenError : Error
{
    public ForbidenError(string message) : base(message)
    {
    }
}

using CursoLog.WebApi.Models.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this ResultBase result, ControllerBase controller)
    {
        var message = new { errors = result.Errors.Select(e => e.Message) };

        if (result.HasError<NotFoundError>())
            return controller.NotFound(message);

        if (result.HasError<ForbidenError>())
            return controller.StatusCode(403, message);

        if (result.HasError<ConflictError>())
            return controller.Conflict(message);

        return controller.BadRequest(message);
    }
}

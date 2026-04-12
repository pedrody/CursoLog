using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(string username, string email, string password);
    Task<Result<string>> LoginAsync(string email, string password);

}

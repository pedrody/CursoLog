using CursoLog.WebApi.DTOs;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> RegisterAsync(string username, string email, string password);
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task<Result<LoginResponse>> RefreshAsync(string refreshToken);

}

using CursoLog.WebApi.Models;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IUserService
{
    Task<Result> CreateAsync(ApplicationUser user, string password);
    Task<Result> AddToRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByRefreshToken(string refreshToken);
    Task<Result> UpdateAsync(ApplicationUser user);
}

using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IUserService
{
    // Identity operations
    Task<Result> CreateAsync(ApplicationUser user, string password);
    Task<Result> AddToRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByRefreshTokenAsync(string refreshToken);
    Task<Result> UpdateAsync(ApplicationUser user);

    // Profile operations
    Task<Result<UserProfileResponse>> GetUserProfileByIdAsync(Guid userId);
    Task<Result> UpdateUserProfileAsync(Guid userId, string name, string? bio, string? profilePictureUrl);
}

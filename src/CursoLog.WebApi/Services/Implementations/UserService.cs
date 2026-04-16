using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CursoLog.WebApi.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> CreateAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> AddToRoleAsync(ApplicationUser user, string role)
    { 
        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }


    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    { 
        var result = await _userManager.GetRolesAsync(user);

        return result;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);

        return result;
    }

    public async Task<ApplicationUser?> FindByRefreshTokenAsync(string refreshToken)
    {
        var result = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        return result;
    }

    public async Task<Result> UpdateAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }
}

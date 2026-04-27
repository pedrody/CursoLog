using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Errors;
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

    // Identity operations

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
    
    // Profile operations

    public async Task<Result<UserProfileResponse>> GetUserProfileByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail(new NotFoundError("User not found"));

        var userProfileResponse = new UserProfileResponse
        (
            user.Id,
            user.UserName!,
            user.Name,
            user.Bio,
            user.ProfilePictureUrl,
            user.RegisteredAt
        );

        return Result.Ok(userProfileResponse);
    }

    public async Task<Result> UpdateUserProfileAsync(Guid userId, string name, string? bio, string? profilePictureUrl)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail(new NotFoundError("User not found"));

        user.Name = name;
        user.Bio = bio;
        user.ProfilePictureUrl = profilePictureUrl;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded 
            ? Result.Ok() 
            : Result.Fail(result.Errors.Select(e => e.Description));
    }
}

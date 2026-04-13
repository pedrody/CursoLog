using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CursoLog.WebApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> RegisterAsync(string username, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            RegisteredAt = DateTime.UtcNow
        };


        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return Result.Fail(errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        string token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Fail("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Result.Fail("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> RefreshAsync(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
            return Result.Fail("Invalid refresh token.");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result.Fail("Refresh token has expired.");

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _tokenService.GenerateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, newRefreshToken);

        return Result.Ok(new LoginResponse(newToken, newRefreshToken));
    }

    private async Task SaveRefreshTokenAsync(ApplicationUser user, string newRefreshToken)
    {
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationInDays"));
        await _userManager.UpdateAsync(user);
    }
}

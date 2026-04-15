using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ISignInService _signInService;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        ITokenService tokenService,
        IConfiguration configuration,
        IUserService userService,
        ISignInService signInService)
    {
        _tokenService = tokenService;
        _configuration = configuration;
        _userService = userService;
        _signInService = signInService;
    }

    public async Task<Result<LoginResponse>> RegisterAsync(string username, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            RegisteredAt = DateTime.UtcNow
        };


        var createResult = await _userService.CreateAsync(user, password);

        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors);

        var addToRoleResult = await _userService.AddToRoleAsync(user, Roles.User);

        if (addToRoleResult.IsFailed)
            return Result.Fail(addToRoleResult.Errors);

        var roles = await _userService.GetRolesAsync(user);
        string token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        var user = await _userService.FindByEmailAsync(email);

        if (user is null)
            return Result.Fail("Invalid credentials.");

        var result = await _signInService.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result)
            return Result.Fail("Invalid credentials.");

        var roles = await _userService.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> RefreshAsync(string refreshToken)
    {
        var user = await _userService.FindByRefreshToken(refreshToken);

        if (user is null)
            return Result.Fail("Invalid refresh token.");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result.Fail("Refresh token has expired.");

        var roles = await _userService.GetRolesAsync(user);
        var newToken = _tokenService.GenerateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, newRefreshToken);

        return Result.Ok(new LoginResponse(newToken, newRefreshToken));
    }

    private async Task SaveRefreshTokenAsync(ApplicationUser user, string newRefreshToken)
    {
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationInDays"));
        
        await _userService.UpdateAsync(user);
    }
}

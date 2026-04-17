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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ITokenService tokenService,
        IConfiguration configuration,
        IUserService userService,
        ISignInService signInService,
        ILogger<AuthService> logger)
    {
        _tokenService = tokenService;
        _configuration = configuration;
        _userService = userService;
        _signInService = signInService;
        _logger = logger;
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
        {
            _logger.LogWarning("Failed to create user {Email}. Reason: {@Errors}.", email, createResult.Errors);
            return Result.Fail(createResult.Errors);
        }

        var addToRoleResult = await _userService.AddToRoleAsync(user, Roles.User);

        if (addToRoleResult.IsFailed)
        {
            _logger.LogWarning("Failed to add user {Email} to role User. Reason: {@Errors}.", email, addToRoleResult.Errors);
            return Result.Fail(addToRoleResult.Errors);
        }

        var roles = await _userService.GetRolesAsync(user);
        string token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        _logger.LogInformation("User {Email} successfully registered and authenticated.", email);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        var user = await _userService.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogWarning("Authentication failed for {Email}. Reason: User not found.", email);
            return Result.Fail("Invalid credentials.");
        }

        var result = await _signInService.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result)
        {
            _logger.LogWarning("Authentication failed for {Email}. Reason: Wrong password.", email);
            return Result.Fail("Invalid credentials.");
        }

        var roles = await _userService.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, refreshToken);

        _logger.LogInformation("User {Email} logged in successfully.", email);

        return Result.Ok(new LoginResponse(token, refreshToken));
    }

    public async Task<Result<LoginResponse>> RefreshAsync(string refreshToken)
    {
        var user = await _userService.FindByRefreshTokenAsync(refreshToken);

        if (user is null)
        {
            _logger.LogWarning("Refresh token failure. Token: {RefreshToken}. Reason: UserNotFound.", refreshToken);
            return Result.Fail("Invalid refresh token.");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token failure for {Email}. Token: {RefreshToken}. Reason: Expired. Expiry: {ExpiryTime} UTC.", 
                user.Email, refreshToken, user.RefreshTokenExpiryTime);
            return Result.Fail("Refresh token has expired.");
        }

        var roles = await _userService.GetRolesAsync(user);
        var newToken = _tokenService.GenerateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user, newRefreshToken);

        _logger.LogInformation("Tokens successfully refreshed for {Email}.", user.Email);

        return Result.Ok(new LoginResponse(newToken, newRefreshToken));
    }

    private async Task SaveRefreshTokenAsync(ApplicationUser user, string newRefreshToken)
    {
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationInDays"));
        
        await _userService.UpdateAsync(user);
    }
}

using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace CursoLog.WebApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> RegisterAsync(string username, string email, string password)
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

        string token = _tokenService.GenerateToken(user, ["User"]);

        return Result.Ok(token);
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Fail("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Result.Fail("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return Result.Ok(token);
    }
}

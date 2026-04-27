using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.username, request.email, request.password);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.email, request.password);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }
}

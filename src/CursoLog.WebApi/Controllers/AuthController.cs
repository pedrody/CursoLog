using CursoLog.WebApi.DTOs;
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
    public async Task<ActionResult<string>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.username, request.email, request.password);

        if (result.IsFailed)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Message) });
        }

        var token = result.Value;

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.email, request.password);

        if (result.IsFailed)
            return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

        var token = result.Value;

        return Ok(new { token });
    }
}

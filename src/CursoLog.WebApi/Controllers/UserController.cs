using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CursoLog.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMe()
    {
        var userId = GetUserIdFromJwtToken();
        var result = await _userService.GetUserProfileByIdAsync(userId);

        return result.IsSuccess ? Ok(result.Value) : result.ToActionResult(this);
    }


    [HttpPut("me")]
    public async Task<ActionResult> UpdateUserProfile([FromBody] UserProfileUpdateRequest userProfile)
    {
        var userId = GetUserIdFromJwtToken();
        var result = await _userService.UpdateUserProfileAsync(userId, userProfile.Name, userProfile.Bio, userProfile.ProfilePictureUrl);

        return result.IsSuccess ? NoContent() : result.ToActionResult(this);
    }
    private Guid GetUserIdFromJwtToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userId!);
    }
}

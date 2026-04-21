using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformService;

    public PlatformController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    [HttpGet("{id}", Name = "GetPlatform")]
    public async Task<ActionResult<PlatformResponse>> GetPlatformByIdAsync(Guid id)
    {
        var result = await _platformService.GetPlatformByIdAsync(id);

        if (result.IsFailed)
            return NotFound();

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<PlatformResponse>> GetAllPlatformsAsync()
    {
        var result = await _platformService.GetAllPlatformsAsync();

        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<PlatformResponse>> AddPlatformAsync([FromBody] PlatformCreateRequest platform)
    {
        var result = await _platformService.AddPlatformAsync(platform.Name, platform.Url);

        if (result.IsFailed)
            return BadRequest();

        return new CreatedAtRouteResult("GetPlatform", new { id = result.Value.Id }, result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePlatformAsync(Guid id)
    {
        var result = await _platformService.DeletePlatformAsync(id);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePlatformAsync(Guid id, [FromBody] PlatformUpdateRequest platform)
    {
        var result = await _platformService.UpdatePlatformAsync(id, platform.Name, platform.Url);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }
}

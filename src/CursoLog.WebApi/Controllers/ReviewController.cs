using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CursoLog.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewByIdAsync(Guid id)
    {
        var result = await _reviewService.GetReviewByIdAsync(id);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpGet("course/{id:guid}")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviewsByCourseIdAsync(Guid id)
    {
        var result = await _reviewService.GetReviewsByCourseIdAsync(id);

        if (result.IsFailed) 
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> AddReview([FromBody] CreateReviewRequest request)
    {
        var userId = GetUserId();
        
        var result = await _reviewService.AddReviewAsync(
            userId, request.courseId, request.rating, request.comment);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
    {
        var userId = GetUserId();

        var result = await _reviewService.UpdateReviewAsync(userId, id, request.rating, request.comment);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteReview(Guid id)
    {
        var userId = GetUserId();

        var result = await _reviewService.DeleteReviewAsync(userId, id);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return NoContent();
    }

    private Guid GetUserId()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        return Guid.Parse(userId);
    }
}

using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewsService _reviewsService;

    public ReviewsController(ReviewsService reviewsService) =>
        _reviewsService = reviewsService;

    [HttpGet]
    public async Task<List<Review>> GetAll([FromQuery] string? movieId = null)
    {
        if (!string.IsNullOrEmpty(movieId))
            return await _reviewsService.GetByMovieIdAsync(movieId);
        return await _reviewsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Review>> GetById(string id)
    {
        var review = await _reviewsService.GetAsync(id);
        if (review is null) return NotFound();
        return review;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Review newReview)
    {
        newReview.CreatedAt = DateTime.UtcNow;
        await _reviewsService.CreateAsync(newReview);
        return CreatedAtAction(nameof(GetById), new { id = newReview.Id }, newReview);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Review updatedReview)
    {
        var existing = await _reviewsService.GetAsync(id);
        if (existing is null) return NotFound();
        updatedReview.Id = existing.Id;
        await _reviewsService.UpdateAsync(id, updatedReview);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var review = await _reviewsService.GetAsync(id);
        if (review is null) return NotFound();
        await _reviewsService.RemoveAsync(id);
        return NoContent();
    }
}
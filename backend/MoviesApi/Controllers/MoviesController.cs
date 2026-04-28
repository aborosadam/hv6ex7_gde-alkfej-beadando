using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MoviesService _moviesService;

    public MoviesController(MoviesService moviesService) =>
        _moviesService = moviesService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var movies = await _moviesService.GetAsync(page, pageSize);
        var totalCount = await _moviesService.GetCountAsync();
        return Ok(new { items = movies, totalCount, page, pageSize });
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Movie>> Get(string id)
    {
        var movie = await _moviesService.GetAsync(id);
        if (movie is null) return NotFound();
        return movie;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Movie newMovie)
    {
        await _moviesService.CreateAsync(newMovie);
        return CreatedAtAction(nameof(Get), new { id = newMovie.Id }, newMovie);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Movie updatedMovie)
    {
        var existing = await _moviesService.GetAsync(id);
        if (existing is null) return NotFound();
        updatedMovie.Id = existing.Id;
        await _moviesService.UpdateAsync(id, updatedMovie);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var movie = await _moviesService.GetAsync(id);
        if (movie is null) return NotFound();
        await _moviesService.RemoveAsync(id);
        return NoContent();
    }
}
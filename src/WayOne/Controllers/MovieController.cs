using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WayOne.Infrastructure;
using WayOne.Models;

namespace WayOne.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _repository;

    public MovieController(IMovieRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
        var movies = await _repository.GetMoviesAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovie([FromRoute] int id)
    {
        var movie = await _repository.GetMovieByIdAsync(id);
        return movie is null ? NotFound() : Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] Movie movie)
    {
        await _repository.AddMovieAsync(movie);
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMovie(int id, Movie movie)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        var updatedRows = await _repository.UpdateMovieAsync(movie);
        if (updatedRows <= 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _repository.GetMovieByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        var deletedRows = await _repository.DeleteMovieAsync(movie);
        if (deletedRows <= 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}
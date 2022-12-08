using Microsoft.EntityFrameworkCore;
using WayTwo.Models;

namespace WayTwo.Infrastructure;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken = default);
    Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken = default);
    Task<int> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<int> AddMovieAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<int> DeleteMovieAsync(Movie movie, CancellationToken cancellationToken = default);
}

public class MovieRepository : IMovieRepository
{
    private readonly MovieDbContext _context;

    public MovieRepository(MovieDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Movies.ToListAsync(cancellationToken);
    }

    public async Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var ids = new object[] { movieId };
        return await _context.Movies.FindAsync(ids, cancellationToken);
    }

    public async Task<int> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Entry(movie).State = EntityState.Modified;
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await MovieExistsAsync(movie.Id);
            if (!exists)
            {
                return -1;
            }
            throw;
        }
    }

    public async Task<int> AddMovieAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        movie.Id = 0;
        await _context.Movies.AddAsync(movie, cancellationToken);
        var rows = await _context.SaveChangesAsync(cancellationToken);
        return rows;
    }

    public async Task<int> DeleteMovieAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _context.Movies.Remove(movie);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> MovieExistsAsync(int id)
    {
        return await _context.Movies.AnyAsync(x => x.Id == id);
    }
}
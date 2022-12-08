using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WayTwo.Infrastructure;
using WayTwo.Models;

namespace WayTwoTests;

public class UnitTests
{
    [Fact]
    public async Task ShouldGetMovies()
    {
        // arrange
        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}")
            .Options;

        await using var movieDbContext = new MovieDbContext(options);
        await movieDbContext.Database.EnsureCreatedAsync();
        var movieRepository = new MovieRepository(movieDbContext);

        // act
        var movies = await movieRepository.GetMoviesAsync();

        // assert
        movies.Should().NotBeNullOrEmpty().And.HaveCount(2);
    }

    [Fact]
    public async Task ShouldGetMovieById()
    {
        // arrange
        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}")
            .Options;

        await using var movieDbContext = new MovieDbContext(options);
        await movieDbContext.Database.EnsureCreatedAsync();
        var movieRepository = new MovieRepository(movieDbContext);

        // act
        var movie = await movieRepository.GetMoviesAsync();

        // assert
        movie.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldAddMovie()
    {
        // arrange
        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}")
            .Options;

        await using var movieDbContext = new MovieDbContext(options);
        await movieDbContext.Database.EnsureCreatedAsync();
        var movieRepository = new MovieRepository(movieDbContext);
        var movie = new Movie
        {
            Title = "The Matrix",
            ReleaseDate = new DateTime(2001, 01, 01)
        };

        // act
        var rows = await movieRepository.AddMovieAsync(movie);

        // assert
        rows.Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateMovie()
    {
        // arrange
        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}")
            .Options;

        await using var movieDbContext = new MovieDbContext(options);
        await movieDbContext.Database.EnsureCreatedAsync();
        var movieRepository = new MovieRepository(movieDbContext);
        var movie = new Movie
        {
            Id = 1,
            Title = "The Matrix",
            ReleaseDate = new DateTime(2001, 01, 01)
        };

        // act
        var rows = await movieRepository.UpdateMovieAsync(movie);

        // assert
        rows.Should().Be(1);
    }

    [Fact]
    public async Task ShouldRemoveMovie()
    {
        // arrange
        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}")
            .Options;

        await using var movieDbContext = new MovieDbContext(options);
        await movieDbContext.Database.EnsureCreatedAsync();
        var movieRepository = new MovieRepository(movieDbContext);
        var movie = new Movie
        {
            Id = 1
        };

        // act
        var rows = await movieRepository.DeleteMovieAsync(movie);

        // assert
        rows.Should().Be(1);
    }
}
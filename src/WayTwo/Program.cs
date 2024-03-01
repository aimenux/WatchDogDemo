using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WatchDog;
using WatchDog.src.Enums;
using WayTwo;
using WayTwo.Infrastructure;
using WayTwo.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var defaultVersion = new ApiVersion(1, 0);
builder.Services
    .AddApiVersioning(config =>
    {
        config.DefaultApiVersion = defaultVersion;
        config.AssumeDefaultVersionWhenUnspecified = true;
        config.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
        options.DefaultApiVersion = defaultVersion;
    });

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddDbContext<MovieDbContext>(options =>
{
    options.UseInMemoryDatabase("MoviesDb");
});

builder.Services.AddWatchDogServices(options =>
{
    options.IsAutoClear = true;
    options.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
});

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
await context.Database.EnsureCreatedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
    });
}

var settings = app.Services.GetRequiredService<IOptions<Settings>>().Value;
if (settings.IsWatchDogEnabled)
{
    app.UseWatchDogExceptionLogger();
    app.UseWatchDog(options =>
    {
        options.WatchPageUsername = settings.WatchPageUsername;
        options.WatchPagePassword = settings.WatchPagePassword;
    });
}

app.UseHttpsRedirection();

var apiVersions = app.NewApiVersionSet()
    .HasApiVersion(defaultVersion)
    .Build();

app
    .MapGet("api/v{version:apiVersion}/movies", async (IMovieRepository repository) =>
    {
        var movies = await repository.GetMoviesAsync();
        return Results.Ok(movies);
    })
    .WithApiVersionSet(apiVersions)
    .WithName("GetMovies");

app
    .MapGet("api/v{version:apiVersion}/movies/{id}", async (int id, IMovieRepository repository) =>
    {
        var movie = await repository.GetMovieByIdAsync(id);
        return Results.Ok(movie);
    })
    .WithApiVersionSet(apiVersions)
    .WithName("GetMovie");

app
    .MapPost("api/v{version:apiVersion}/movies", async (Movie movie, IMovieRepository repository) =>
    {
        await repository.AddMovieAsync(movie);
        return Results.CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
    })
    .WithApiVersionSet(apiVersions)
    .WithName("AddMovie");

app
    .MapPut("api/v{version:apiVersion}/movies/{id}", async (int id, Movie movie, IMovieRepository repository) =>
    {
        if (id != movie.Id)
        {
            return Results.BadRequest();
        }

        var updatedRows = await repository.UpdateMovieAsync(movie);
        if (updatedRows <= 0)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    })
    .WithApiVersionSet(apiVersions)
    .WithName("UpdateMovie");

app
    .MapDelete("api/v{version:apiVersion}/movies/{id}", async (int id, IMovieRepository repository) =>
    {
        var movie = await repository.GetMovieByIdAsync(id);
        if (movie is null)
        {
            return Results.NotFound();
        }

        var deletedRows = await repository.DeleteMovieAsync(movie);
        if (deletedRows <= 0)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    })
    .WithApiVersionSet(apiVersions)
    .WithName("DeleteMovie");

app.Run();
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WatchDog;
using WatchDog.src.Enums;
using WayOne;
using WayOne.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
    .AddApiExplorer(o =>
    {
        o.GroupNameFormat = "'v'VVV";
        o.SubstituteApiVersionInUrl = true;
        o.DefaultApiVersion = defaultVersion;
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

app.UseAuthorization();

app.MapControllers();

app.Run();
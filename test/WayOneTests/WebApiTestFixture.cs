using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WayOne.Infrastructure;

namespace WayOneTests;

internal class WebApiTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.testing.json");
            configBuilder.AddJsonFile(configPath);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<MovieDbContext>(options =>
            {
                options.UseInMemoryDatabase($"MoviesDb-{Guid.NewGuid():N}");
            });
        });
    }
}
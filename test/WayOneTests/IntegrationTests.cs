using System.Net;
using System.Text;
using FluentAssertions;

namespace WayOneTests;

public class IntegrationTests
{
    [Fact]
    public async Task ShouldGetMovies()
    {
        // arrange
        await using var factory = new IntegrationWebApplicationFactory();
        using var client = factory.CreateClient();

        // act
        var response = await client.GetAsync("/api/v1/movies");
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ShouldGetMovieById()
    {
        // arrange
        await using var factory = new IntegrationWebApplicationFactory();
        using var client = factory.CreateClient();

        // act
        var response = await client.GetAsync("/api/v1/movies/1");
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ShouldAddMovie()
    {
        // arrange
        await using var factory = new IntegrationWebApplicationFactory();
        using var client = factory.CreateClient();
        const string payload = "{\"title\": \"The Matrix\",\"ReleaseDate\": \"1999-03-24T12:14:09.599Z\"}";
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // act
        var response = await client.PostAsync("/api/v1/movies", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseBody.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ShouldUpdateMovie()
    {
        // arrange
        await using var factory = new IntegrationWebApplicationFactory();
        using var client = factory.CreateClient();
        const string payload = "{\"id\": 1,\"title\": \"Titanic\",\"ReleaseDate\": \"1998-01-15T12:14:09.599Z\"}";
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync("/api/v1/movies/1", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        responseBody.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldDeleteMovie()
    {
        // arrange
        await using var factory = new IntegrationWebApplicationFactory();
        using var client = factory.CreateClient();

        // act
        var response = await client.DeleteAsync("/api/v1/movies/2");
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        responseBody.Should().BeEmpty();
    }
}
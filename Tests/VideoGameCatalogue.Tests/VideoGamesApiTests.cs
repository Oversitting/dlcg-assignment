using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VideoGameCatalogue.Api;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Data;
using VideoGameCatalogue.Api.Implementation.Models;

namespace VideoGameCatalogue.Tests;

public sealed class VideoGamesApiTests
{
    [Fact]
    public async Task PostAsync_ReturnsValidationProblem_WhenRequestIsInvalid()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/video-games", new SaveVideoGameRequest
        {
            Title = string.Empty,
            Genre = "Action",
            Platform = "PC",
            ReleaseYear = 1990,
            Developer = "Studio",
            Publisher = string.Empty,
            CriticScore = 80,
            Summary = null
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Contains("Title is required.", problem.Errors[nameof(SaveVideoGameRequest.Title)]);
        Assert.Contains("Publisher is required.", problem.Errors[nameof(SaveVideoGameRequest.Publisher)]);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProblemDetails_WhenEntryDoesNotExist()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/video-games/404");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal("Resource not found.", problem.Title);
        Assert.Equal("Video game with id 404 was not found.", problem.Detail);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_AndRemovesEntry()
    {
        var seededGame = new VideoGame
        {
            Id = 42,
            Title = "Sea of Stars",
            Genre = "RPG",
            Platform = "PC",
            ReleaseYear = 2023,
            Developer = "Sabotage Studio",
            Publisher = "Sabotage Studio",
            CriticScore = 89,
            UpdatedUtc = DateTime.UtcNow
        };

        await using var factory = CreateFactory(seededGame);
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/video-games/42");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();
        var exists = await dbContext.VideoGames.AnyAsync(item => item.Id == 42);

        Assert.False(exists);
    }

    private static WebApplicationFactory<Program> CreateFactory(params VideoGame[] seedData)
    {
        var databaseName = Guid.NewGuid().ToString();

        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<CatalogueDbContext>>();
                services.RemoveAll<CatalogueDbContext>();

                services.AddDbContext<CatalogueDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));

                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();

                dbContext.Database.EnsureCreated();

                if (seedData.Length > 0)
                {
                    dbContext.VideoGames.AddRange(seedData);
                    dbContext.SaveChanges();
                }
            });
        });
    }
}
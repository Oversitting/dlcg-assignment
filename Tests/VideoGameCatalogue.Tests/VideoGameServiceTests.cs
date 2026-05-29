using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Data;
using VideoGameCatalogue.Api.Implementation.Exceptions;
using VideoGameCatalogue.Api.Implementation.Models;
using VideoGameCatalogue.Api.Implementation.Services;

namespace VideoGameCatalogue.Tests;

public sealed class VideoGameServiceTests
{
    [Fact]
    public async Task GetAllAsync_AppliesSearchAndPlatformFilters()
    {
        await using var dbContext = CreateDbContext();
        dbContext.VideoGames.AddRange(
            new VideoGame
            {
                Title = "Halo Infinite",
                Genre = "Shooter",
                Platform = "Xbox Series X|S",
                ReleaseYear = 2021,
                Developer = "343 Industries",
                Publisher = "Xbox Game Studios",
                CriticScore = 87,
                UpdatedUtc = DateTime.UtcNow
            },
            new VideoGame
            {
                Title = "Halo: The Master Chief Collection",
                Genre = "Shooter",
                Platform = "PC",
                ReleaseYear = 2019,
                Developer = "343 Industries",
                Publisher = "Xbox Game Studios",
                CriticScore = 86,
                UpdatedUtc = DateTime.UtcNow
            },
            new VideoGame
            {
                Title = "Cocoon",
                Genre = "Puzzle",
                Platform = "PC",
                ReleaseYear = 2023,
                Developer = "Geometric Interactive",
                Publisher = "Annapurna Interactive",
                CriticScore = 88,
                UpdatedUtc = DateTime.UtcNow
            });

        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var request = new VideoGameBrowseRequest
        {
            SearchTerm = "Halo",
            Platform = "PC"
        };

        var result = await service.GetAllAsync(request);

        Assert.Equal(1, result.TotalCount);
        var videoGame = Assert.Single(result.Items);
        Assert.Equal("Halo: The Master Chief Collection", videoGame.Title);
    }

    [Fact]
    public async Task GetAllAsync_AppliesOrderingAndPagination()
    {
        await using var dbContext = CreateDbContext();
        dbContext.VideoGames.AddRange(
            new VideoGame
            {
                Title = "Game A",
                Genre = "Action",
                Platform = "PC",
                ReleaseYear = 2020,
                Developer = "Studio A",
                Publisher = "Publisher A",
                CriticScore = 82,
                UpdatedUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new VideoGame
            {
                Title = "Game B",
                Genre = "Action",
                Platform = "PC",
                ReleaseYear = 2024,
                Developer = "Studio B",
                Publisher = "Publisher B",
                CriticScore = 95,
                UpdatedUtc = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new VideoGame
            {
                Title = "Game C",
                Genre = "Action",
                Platform = "PC",
                ReleaseYear = 2022,
                Developer = "Studio C",
                Publisher = "Publisher C",
                CriticScore = 90,
                UpdatedUtc = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            });

        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var request = new VideoGameBrowseRequest
        {
            OrderBy = VideoGameBrowseOrderBy.CriticScore,
            OrderDirection = VideoGameBrowseOrderDirection.Descending,
            PageNumber = 2,
            PageSize = 1
        };

        var result = await service.GetAllAsync(request);

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(1, result.PageSize);
        var videoGame = Assert.Single(result.Items);
        Assert.Equal("Game C", videoGame.Title);
    }

    [Fact]
    public async Task GetAllAsync_DefaultsToIdOrdering()
    {
        await using var dbContext = CreateDbContext();
        dbContext.VideoGames.AddRange(
            new VideoGame
            {
                Title = "Newest",
                Genre = "Action",
                Platform = "PC",
                ReleaseYear = 2024,
                Developer = "Studio B",
                Publisher = "Publisher B",
                CriticScore = 90,
                UpdatedUtc = DateTime.UtcNow
            },
            new VideoGame
            {
                Title = "Oldest",
                Genre = "Action",
                Platform = "PC",
                ReleaseYear = 2020,
                Developer = "Studio A",
                Publisher = "Publisher A",
                CriticScore = 80,
                UpdatedUtc = DateTime.UtcNow
            });

        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var result = await service.GetAllAsync(new VideoGameBrowseRequest());

        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Newest", result.Items[0].Title);
        Assert.Equal("Oldest", result.Items[1].Title);
    }

    [Fact]
    public async Task GetAllAsync_UsesStableOrderingWhenPrimarySortValuesMatch()
    {
        await using var dbContext = CreateDbContext();
        dbContext.VideoGames.AddRange(
            new VideoGame
            {
                Title = "Shared Title",
                Genre = "Adventure",
                Platform = "PC",
                ReleaseYear = 2024,
                Developer = "Studio A",
                Publisher = "Publisher A",
                CriticScore = 90,
                UpdatedUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new VideoGame
            {
                Title = "Shared Title",
                Genre = "Adventure",
                Platform = "PC",
                ReleaseYear = 2024,
                Developer = "Studio B",
                Publisher = "Publisher B",
                CriticScore = 90,
                UpdatedUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var firstPage = await service.GetAllAsync(new VideoGameBrowseRequest
        {
            SearchTerm = "Shared Title",
            OrderBy = VideoGameBrowseOrderBy.CriticScore,
            OrderDirection = VideoGameBrowseOrderDirection.Descending,
            PageNumber = 1,
            PageSize = 1
        });

        var secondPage = await service.GetAllAsync(new VideoGameBrowseRequest
        {
            SearchTerm = "Shared Title",
            OrderBy = VideoGameBrowseOrderBy.CriticScore,
            OrderDirection = VideoGameBrowseOrderDirection.Descending,
            PageNumber = 2,
            PageSize = 1
        });

        Assert.Single(firstPage.Items);
        Assert.Single(secondPage.Items);
        Assert.NotEqual(firstPage.Items[0].Id, secondPage.Items[0].Id);
    }

    [Fact]
    public async Task UpdateAsync_OverwritesFieldsAndRefreshesTimestamp()
    {
        await using var dbContext = CreateDbContext();
        var existing = new VideoGame
        {
            Title = "Control",
            Genre = "Action Adventure",
            Platform = "PC",
            ReleaseYear = 2019,
            Developer = "Remedy Entertainment",
            Publisher = "505 Games",
            CriticScore = 84,
            UpdatedUtc = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        dbContext.VideoGames.Add(existing);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var request = new SaveVideoGameRequest
        {
            Title = "Control Ultimate Edition",
            Genre = "Action Adventure",
            Platform = "PlayStation 5",
            ReleaseYear = 2021,
            Developer = "Remedy Entertainment",
            Publisher = "505 Games",
            CriticScore = 85,
            Summary = "Enhanced edition for newer consoles."
        };

        var result = await service.UpdateAsync(existing.Id, request);

        Assert.NotNull(result);
        Assert.Equal("Control Ultimate Edition", result.Title);
        Assert.Equal("PlayStation 5", result.Platform);
        Assert.True(result.UpdatedUtc > new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsWhenVideoGameDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var action = () => service.GetByIdAsync(404);

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(action);
        Assert.Equal("Video game with id 404 was not found.", exception.Message);
    }

    private static CatalogueDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CatalogueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CatalogueDbContext(options);
    }

    private static VideoGameService CreateService(CatalogueDbContext dbContext)
    {
        return new VideoGameService(dbContext, NullLogger<VideoGameService>.Instance);
    }
}
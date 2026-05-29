using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Data;
using VideoGameCatalogue.Api.Implementation.Exceptions;
using VideoGameCatalogue.Api.Implementation.Models;

namespace VideoGameCatalogue.Api.Implementation.Services;

public sealed class VideoGameService : IVideoGameService
{
    private readonly CatalogueDbContext _dbContext;
    private readonly ILogger<VideoGameService> _logger;

    public VideoGameService(
        CatalogueDbContext dbContext,
        ILogger<VideoGameService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<PagedResponse<VideoGameResponse>> GetAllAsync(
        VideoGameBrowseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _dbContext.VideoGames.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            var pattern = $"%{searchTerm}%";

            query = query.Where(videoGame =>
                EF.Functions.Like(videoGame.Title, pattern) ||
                EF.Functions.Like(videoGame.Developer, pattern) ||
                EF.Functions.Like(videoGame.Publisher, pattern));
        }

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            var genre = request.Genre.Trim();
            query = query.Where(videoGame => videoGame.Genre == genre);
        }

        if (!string.IsNullOrWhiteSpace(request.Platform))
        {
            var platform = request.Platform.Trim();
            query = query.Where(videoGame => videoGame.Platform == platform);
        }

        query = ApplyOrdering(query, request.OrderBy, request.OrderDirection);

        var totalCount = await query.CountAsync(cancellationToken);
        var videoGames = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(videoGame => Map(videoGame))
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved page {PageNumber} of video games with page size {PageSize}. Total matching entries: {TotalCount}. SearchTerm '{SearchTerm}', genre '{Genre}', platform '{Platform}', orderBy '{OrderBy}', orderDirection '{OrderDirection}'.",
            request.PageNumber,
            request.PageSize,
            totalCount,
            request.SearchTerm?.Trim(),
            request.Genre?.Trim(),
            request.Platform?.Trim(),
            request.OrderBy,
            request.OrderDirection);

        return new PagedResponse<VideoGameResponse>(videoGames, totalCount, request.PageNumber, request.PageSize);
    }

    /// <inheritdoc />
    public async Task<VideoGameResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var videoGame = await _dbContext.VideoGames
            .AsNoTracking()
            .Where(videoGame => videoGame.Id == id)
            .Select(videoGame => Map(videoGame))
            .SingleOrDefaultAsync(cancellationToken);

        if (videoGame is null)
        {
            _logger.LogWarning("Video game {VideoGameId} was not found.", id);
            throw new ResourceNotFoundException($"Video game with id {id} was not found.");
        }

        _logger.LogInformation("Retrieved video game {VideoGameId}.", id);
        return videoGame;
    }

    /// <inheritdoc />
    public async Task<VideoGameResponse> CreateAsync(
        SaveVideoGameRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var videoGame = new VideoGame();
        Apply(videoGame, request);

        _dbContext.VideoGames.Add(videoGame);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created video game {VideoGameId} with title '{Title}'.",
            videoGame.Id,
            videoGame.Title);

        return Map(videoGame);
    }

    /// <inheritdoc />
    public async Task<VideoGameResponse> UpdateAsync(
        int id,
        SaveVideoGameRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var videoGame = await _dbContext.VideoGames.SingleOrDefaultAsync(
            item => item.Id == id,
            cancellationToken);

        if (videoGame is null)
        {
            _logger.LogWarning("Video game {VideoGameId} was not found for update.", id);
            throw new ResourceNotFoundException($"Video game with id {id} was not found.");
        }

        Apply(videoGame, request);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated video game {VideoGameId} with title '{Title}'.",
            videoGame.Id,
            videoGame.Title);

        return Map(videoGame);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var videoGame = await _dbContext.VideoGames.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (videoGame is null)
        {
            _logger.LogWarning("Video game {VideoGameId} was not found for deletion.", id);
            throw new ResourceNotFoundException($"Video game with id {id} was not found.");
        }

        _dbContext.VideoGames.Remove(videoGame);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted video game {VideoGameId} with title '{Title}'.", videoGame.Id, videoGame.Title);
    }

    private static void Apply(VideoGame videoGame, SaveVideoGameRequest request)
    {
        videoGame.Title = request.Title.Trim();
        videoGame.Genre = request.Genre.Trim();
        videoGame.Platform = request.Platform.Trim();
        videoGame.ReleaseYear = request.ReleaseYear;
        videoGame.Developer = request.Developer.Trim();
        videoGame.Publisher = request.Publisher.Trim();
        videoGame.CriticScore = request.CriticScore;
        videoGame.Summary = string.IsNullOrWhiteSpace(request.Summary) ? null : request.Summary.Trim();
        videoGame.UpdatedUtc = DateTime.UtcNow;
    }

    private static IQueryable<VideoGame> ApplyOrdering(
        IQueryable<VideoGame> query,
        string orderBy,
        string orderDirection)
    {
        var descending = string.Equals(orderDirection, VideoGameBrowseOrderDirection.Descending, StringComparison.OrdinalIgnoreCase);

        return (orderBy, descending) switch
        {
            (VideoGameBrowseOrderBy.Id, true) => query.OrderByDescending(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.Id, false) => query.OrderBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.ReleaseYear, true) => query.OrderByDescending(videoGame => videoGame.ReleaseYear).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.ReleaseYear, false) => query.OrderBy(videoGame => videoGame.ReleaseYear).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.CriticScore, true) => query.OrderByDescending(videoGame => videoGame.CriticScore).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.CriticScore, false) => query.OrderBy(videoGame => videoGame.CriticScore).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.UpdatedUtc, true) => query.OrderByDescending(videoGame => videoGame.UpdatedUtc).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.UpdatedUtc, false) => query.OrderBy(videoGame => videoGame.UpdatedUtc).ThenBy(videoGame => videoGame.Id),
            (VideoGameBrowseOrderBy.Title, true) => query.OrderByDescending(videoGame => videoGame.Title).ThenBy(videoGame => videoGame.Id),
            _ => query.OrderBy(videoGame => videoGame.Title).ThenBy(videoGame => videoGame.Id),
        };
    }

    private static VideoGameResponse Map(VideoGame videoGame)
    {
        return new VideoGameResponse(
            videoGame.Id,
            videoGame.Title,
            videoGame.Genre,
            videoGame.Platform,
            videoGame.ReleaseYear,
            videoGame.Developer,
            videoGame.Publisher,
            videoGame.CriticScore,
            videoGame.Summary,
            videoGame.UpdatedUtc);
    }
}
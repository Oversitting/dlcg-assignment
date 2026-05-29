namespace VideoGameCatalogue.Api.Contracts;

using VideoGameCatalogue.Api.Implementation.Models;

/// <summary>
/// Represents the query string parameters used to browse catalogue entries.
/// </summary>
public sealed class VideoGameBrowseRequest
{
    /// <summary>
    /// Gets the free-text term matched against title, developer, and publisher.
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Gets the genre filter applied to the catalogue results.
    /// </summary>
    public string? Genre { get; init; }

    /// <summary>
    /// Gets the platform filter applied to the catalogue results.
    /// </summary>
    public string? Platform { get; init; }

    /// <summary>
    /// Gets the field used to order the browse results.
    /// </summary>
    public string OrderBy { get; init; } = VideoGameBrowseOrderBy.Id;

    /// <summary>
    /// Gets the ordering direction for the browse results.
    /// </summary>
    public string OrderDirection { get; init; } = VideoGameBrowseOrderDirection.Ascending;

    /// <summary>
    /// Gets the one-based page number to retrieve.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets the number of catalogue entries returned per page.
    /// </summary>
    public int PageSize { get; init; } = VideoGameConstraints.DefaultPageSize;
}

/// <summary>
/// Defines the supported browse sort fields.
/// </summary>
public static class VideoGameBrowseOrderBy
{
    /// <summary>
    /// Sorts results by identifier.
    /// </summary>
    public const string Id = "id";

    /// <summary>
    /// Sorts results by title.
    /// </summary>
    public const string Title = "title";

    /// <summary>
    /// Sorts results by release year.
    /// </summary>
    public const string ReleaseYear = "releaseYear";

    /// <summary>
    /// Sorts results by critic score.
    /// </summary>
    public const string CriticScore = "criticScore";

    /// <summary>
    /// Sorts results by last update timestamp.
    /// </summary>
    public const string UpdatedUtc = "updatedUtc";
}

/// <summary>
/// Defines the supported browse sort directions.
/// </summary>
public static class VideoGameBrowseOrderDirection
{
    /// <summary>
    /// Sorts results in ascending order.
    /// </summary>
    public const string Ascending = "asc";

    /// <summary>
    /// Sorts results in descending order.
    /// </summary>
    public const string Descending = "desc";
}
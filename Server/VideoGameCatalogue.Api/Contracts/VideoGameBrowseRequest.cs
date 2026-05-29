namespace VideoGameCatalogue.Api.Contracts;

using VideoGameCatalogue.Api.Implementation.Models;

public sealed class VideoGameBrowseRequest
{
    public string? SearchTerm { get; init; }

    public string? Genre { get; init; }

    public string? Platform { get; init; }

    public string OrderBy { get; init; } = VideoGameBrowseOrderBy.Id;

    public string OrderDirection { get; init; } = VideoGameBrowseOrderDirection.Ascending;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = VideoGameConstraints.DefaultPageSize;
}

public static class VideoGameBrowseOrderBy
{
    public const string Id = "id";
    public const string Title = "title";
    public const string ReleaseYear = "releaseYear";
    public const string CriticScore = "criticScore";
    public const string UpdatedUtc = "updatedUtc";
}

public static class VideoGameBrowseOrderDirection
{
    public const string Ascending = "asc";
    public const string Descending = "desc";
}
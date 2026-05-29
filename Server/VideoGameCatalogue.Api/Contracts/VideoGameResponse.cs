namespace VideoGameCatalogue.Api.Contracts;

/// <summary>
/// Represents a single catalogue entry returned by the API.
/// </summary>
/// <param name="Id">The unique identifier of the catalogue entry.</param>
/// <param name="Title">The display title of the video game.</param>
/// <param name="Genre">The primary genre label.</param>
/// <param name="Platform">The platform label shown in the catalogue.</param>
/// <param name="ReleaseYear">The year the game was released.</param>
/// <param name="Developer">The studio that developed the game.</param>
/// <param name="Publisher">The publisher credited for the release.</param>
/// <param name="CriticScore">The critic score stored in the catalogue.</param>
/// <param name="Summary">The optional summary for the catalogue entry.</param>
/// <param name="UpdatedUtc">The UTC timestamp of the most recent update.</param>
public sealed record VideoGameResponse(
    int Id,
    string Title,
    string Genre,
    string Platform,
    int ReleaseYear,
    string Developer,
    string Publisher,
    int CriticScore,
    string? Summary,
    DateTime UpdatedUtc);

/// <summary>
/// Represents a single page of results returned from a browse operation.
/// </summary>
/// <typeparam name="T">The item type contained in the page.</typeparam>
/// <param name="Items">The items returned for the current page.</param>
/// <param name="TotalCount">The total number of matching items across all pages.</param>
/// <param name="PageNumber">The one-based page number returned.</param>
/// <param name="PageSize">The requested page size.</param>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
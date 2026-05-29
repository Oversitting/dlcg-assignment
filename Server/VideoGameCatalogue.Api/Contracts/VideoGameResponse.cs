namespace VideoGameCatalogue.Api.Contracts;

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

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
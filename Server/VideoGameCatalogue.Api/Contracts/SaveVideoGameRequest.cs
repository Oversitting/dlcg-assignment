namespace VideoGameCatalogue.Api.Contracts;

public sealed class SaveVideoGameRequest
{
    public string Title { get; init; } = string.Empty;

    public string Genre { get; init; } = string.Empty;

    public string Platform { get; init; } = string.Empty;

    public int ReleaseYear { get; init; }

    public string Developer { get; init; } = string.Empty;

    public string Publisher { get; init; } = string.Empty;

    public int CriticScore { get; init; }

    public string? Summary { get; init; }
}
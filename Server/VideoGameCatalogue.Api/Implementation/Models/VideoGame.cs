namespace VideoGameCatalogue.Api.Implementation.Models;

public sealed class VideoGame
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    public string Developer { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public int CriticScore { get; set; }

    public string? Summary { get; set; }

    public DateTime UpdatedUtc { get; set; }
}
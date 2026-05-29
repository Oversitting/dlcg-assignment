namespace VideoGameCatalogue.Api.Contracts;

/// <summary>
/// Represents the request body used to create or update a catalogue entry.
/// </summary>
public sealed class SaveVideoGameRequest
{
    /// <summary>
    /// Gets the display title of the video game.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the primary genre label used for browsing.
    /// </summary>
    public string Genre { get; init; } = string.Empty;

    /// <summary>
    /// Gets the platform label shown in the catalogue.
    /// </summary>
    public string Platform { get; init; } = string.Empty;

    /// <summary>
    /// Gets the release year used for filtering and sorting.
    /// </summary>
    public int ReleaseYear { get; init; }

    /// <summary>
    /// Gets the studio that developed the game.
    /// </summary>
    public string Developer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the publisher credited for the release.
    /// </summary>
    public string Publisher { get; init; } = string.Empty;

    /// <summary>
    /// Gets the critic score stored in the catalogue.
    /// </summary>
    public int CriticScore { get; init; }

    /// <summary>
    /// Gets the optional summary displayed for the catalogue entry.
    /// </summary>
    public string? Summary { get; init; }
}
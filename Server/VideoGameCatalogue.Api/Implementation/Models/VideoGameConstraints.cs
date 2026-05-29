namespace VideoGameCatalogue.Api.Implementation.Models;

public static class VideoGameConstraints
{
    public const int TitleMaxLength = 120;
    public const int GenreMaxLength = 60;
    public const int PlatformMaxLength = 60;
    public const int DeveloperMaxLength = 120;
    public const int PublisherMaxLength = 120;
    public const int SummaryMaxLength = 500;
    public const int MinReleaseYear = 1970;
    public const int MaxReleaseYear = 2100;
    public const int MinCriticScore = 0;
    public const int MaxCriticScore = 100;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
}
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Api.Implementation.Models;

namespace VideoGameCatalogue.Api.Data;

public sealed class CatalogueDbContext : DbContext
{
    public CatalogueDbContext(DbContextOptions<CatalogueDbContext> options)
        : base(options)
    {
    }

    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var videoGame = modelBuilder.Entity<VideoGame>();

        videoGame.ToTable("VideoGames");
        videoGame.HasKey(item => item.Id);
        videoGame.HasIndex(item => item.Title);
        videoGame.Property(item => item.Title).HasMaxLength(VideoGameConstraints.TitleMaxLength).IsRequired();
        videoGame.Property(item => item.Genre).HasMaxLength(VideoGameConstraints.GenreMaxLength).IsRequired();
        videoGame.Property(item => item.Platform).HasMaxLength(VideoGameConstraints.PlatformMaxLength).IsRequired();
        videoGame.Property(item => item.ReleaseYear).IsRequired();
        videoGame.Property(item => item.Developer).HasMaxLength(VideoGameConstraints.DeveloperMaxLength).IsRequired();
        videoGame.Property(item => item.Publisher).HasMaxLength(VideoGameConstraints.PublisherMaxLength).IsRequired();
        videoGame.Property(item => item.CriticScore).IsRequired();
        videoGame.Property(item => item.Summary).HasMaxLength(VideoGameConstraints.SummaryMaxLength);
        videoGame.Property(item => item.UpdatedUtc).IsRequired();
    }
}
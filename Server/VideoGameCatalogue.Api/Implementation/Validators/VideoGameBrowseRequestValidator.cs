using FluentValidation;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Implementation.Models;

namespace VideoGameCatalogue.Api.Implementation.Validators;

public sealed class VideoGameBrowseRequestValidator : AbstractValidator<VideoGameBrowseRequest>
{
    public VideoGameBrowseRequestValidator()
    {
        RuleFor(request => request.SearchTerm)
            .MaximumLength(VideoGameConstraints.TitleMaxLength);

        RuleFor(request => request.Genre)
            .MaximumLength(VideoGameConstraints.GenreMaxLength);

        RuleFor(request => request.Platform)
            .MaximumLength(VideoGameConstraints.PlatformMaxLength);

        RuleFor(request => request.OrderBy)
            .Must(orderBy => orderBy is VideoGameBrowseOrderBy.Id or VideoGameBrowseOrderBy.Title or VideoGameBrowseOrderBy.ReleaseYear or VideoGameBrowseOrderBy.CriticScore or VideoGameBrowseOrderBy.UpdatedUtc)
            .WithMessage("OrderBy must be one of: id, title, releaseYear, criticScore, updatedUtc.");

        RuleFor(request => request.OrderDirection)
            .Must(orderDirection => orderDirection is VideoGameBrowseOrderDirection.Ascending or VideoGameBrowseOrderDirection.Descending)
            .WithMessage("OrderDirection must be either asc or desc.");

        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, VideoGameConstraints.MaxPageSize);
    }
}
using FluentValidation;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Implementation.Models;

namespace VideoGameCatalogue.Api.Implementation.Validators;

public sealed class SaveVideoGameRequestValidator : AbstractValidator<SaveVideoGameRequest>
{
    public SaveVideoGameRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(VideoGameConstraints.TitleMaxLength);

        RuleFor(request => request.Genre)
            .NotEmpty()
            .WithMessage("Genre is required.")
            .MaximumLength(VideoGameConstraints.GenreMaxLength);

        RuleFor(request => request.Platform)
            .NotEmpty()
            .WithMessage("Platform is required.")
            .MaximumLength(VideoGameConstraints.PlatformMaxLength);

        RuleFor(request => request.ReleaseYear)
            .InclusiveBetween(VideoGameConstraints.MinReleaseYear, VideoGameConstraints.MaxReleaseYear);

        RuleFor(request => request.Developer)
            .NotEmpty()
            .WithMessage("Developer is required.")
            .MaximumLength(VideoGameConstraints.DeveloperMaxLength);

        RuleFor(request => request.Publisher)
            .NotEmpty()
            .WithMessage("Publisher is required.")
            .MaximumLength(VideoGameConstraints.PublisherMaxLength);

        RuleFor(request => request.CriticScore)
            .InclusiveBetween(VideoGameConstraints.MinCriticScore, VideoGameConstraints.MaxCriticScore);

        RuleFor(request => request.Summary)
            .MaximumLength(VideoGameConstraints.SummaryMaxLength);
    }
}
using FluentValidation.TestHelper;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Implementation.Validators;

namespace VideoGameCatalogue.Tests;

public sealed class SaveVideoGameRequestValidatorTests
{
    private readonly SaveVideoGameRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenRequiredFieldsAreWhitespace()
    {
        var request = new SaveVideoGameRequest
        {
            Title = "   ",
            Genre = " ",
            Platform = "PC",
            ReleaseYear = 2024,
            Developer = " ",
            Publisher = "Sega",
            CriticScore = 90,
            Summary = "Fantasy role-playing game."
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(item => item.Title);
        result.ShouldHaveValidationErrorFor(item => item.Genre);
        result.ShouldHaveValidationErrorFor(item => item.Developer);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNumericFieldsAreOutOfRange()
    {
        var request = new SaveVideoGameRequest
        {
            Title = "Metaphor: ReFantazio",
            Genre = "RPG",
            Platform = "PC",
            ReleaseYear = 1960,
            Developer = "Studio Zero",
            Publisher = "Sega",
            CriticScore = 101,
            Summary = new string('x', 501)
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(item => item.ReleaseYear);
        result.ShouldHaveValidationErrorFor(item => item.CriticScore);
        result.ShouldHaveValidationErrorFor(item => item.Summary);
    }
}
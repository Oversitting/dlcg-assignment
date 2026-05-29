using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Implementation.Services;

namespace VideoGameCatalogue.Api.Controllers;

[ApiController]
[Route("api/video-games")]
public sealed class VideoGamesController : ControllerBase
{
    private readonly IVideoGameService _videoGameService;

    public VideoGamesController(IVideoGameService videoGameService)
    {
        _videoGameService = videoGameService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<VideoGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<VideoGameResponse>>> GetAll(
        [FromQuery] VideoGameBrowseRequest request,
        CancellationToken cancellationToken)
    {
        var videoGames = await _videoGameService.GetAllAsync(request, cancellationToken);
        return Ok(videoGames);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VideoGameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VideoGameResponse>> GetById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var videoGame = await _videoGameService.GetByIdAsync(id, cancellationToken);
        return Ok(videoGame);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VideoGameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VideoGameResponse>> Create(
        [FromBody] SaveVideoGameRequest request,
        CancellationToken cancellationToken)
    {
        var videoGame = await _videoGameService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = videoGame.Id }, videoGame);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VideoGameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VideoGameResponse>> Update(
        [Range(1, int.MaxValue)] int id,
        [FromBody] SaveVideoGameRequest request,
        CancellationToken cancellationToken)
    {
        var videoGame = await _videoGameService.UpdateAsync(id, request, cancellationToken);
        return Ok(videoGame);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        await _videoGameService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
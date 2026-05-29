using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Implementation.Services;

namespace VideoGameCatalogue.Api.Controllers;

/// <summary>
/// Exposes browse and CRUD operations for catalogue video game entries.
/// </summary>
[ApiController]
[Route("api/video-games")]
public sealed class VideoGamesController : ControllerBase
{
    private readonly IVideoGameService _videoGameService;

    /// <summary>
    /// Creates a new controller instance.
    /// </summary>
    /// <param name="videoGameService">The service that handles catalogue operations.</param>
    public VideoGamesController(IVideoGameService videoGameService)
    {
        _videoGameService = videoGameService;
    }

    /// <summary>
    /// Retrieves a paged set of catalogue entries using the requested filters, sort order, and page settings.
    /// </summary>
    /// <param name="request">The browse filters, ordering, and pagination options.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>A paged response containing the matching catalogue entries.</returns>
    /// <response code="200">Returns the requested page of catalogue entries.</response>
    /// <response code="400">The query string parameters were invalid.</response>
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

    /// <summary>
    /// Retrieves a single catalogue entry by its identifier.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The requested catalogue entry.</returns>
    /// <response code="200">Returns the requested catalogue entry.</response>
    /// <response code="400">The supplied identifier was invalid.</response>
    /// <response code="404">No catalogue entry exists for the supplied identifier.</response>
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

    /// <summary>
    /// Creates a new catalogue entry.
    /// </summary>
    /// <param name="request">The catalogue entry values to create.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The created catalogue entry.</returns>
    /// <response code="201">Creates the catalogue entry and returns the created resource.</response>
    /// <response code="400">The submitted request body was invalid.</response>
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

    /// <summary>
    /// Updates an existing catalogue entry.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="request">The updated catalogue entry values.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The updated catalogue entry.</returns>
    /// <response code="200">Returns the updated catalogue entry.</response>
    /// <response code="400">The supplied identifier or request body was invalid.</response>
    /// <response code="404">No catalogue entry exists for the supplied identifier.</response>
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

    /// <summary>
    /// Deletes an existing catalogue entry.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>A no-content result when the delete succeeds.</returns>
    /// <response code="204">The catalogue entry was deleted.</response>
    /// <response code="400">The supplied identifier was invalid.</response>
    /// <response code="404">No catalogue entry exists for the supplied identifier.</response>
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
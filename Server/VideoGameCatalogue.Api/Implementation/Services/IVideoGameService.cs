using VideoGameCatalogue.Api.Contracts;

namespace VideoGameCatalogue.Api.Implementation.Services;

/// <summary>
/// Defines the application operations for browsing and mutating catalogue entries.
/// </summary>
public interface IVideoGameService
{
    /// <summary>
    /// Retrieves a paged list of catalogue entries that match the supplied browse request.
    /// </summary>
    /// <param name="request">The browse filters, ordering, and pagination options.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>A paged response containing the matching catalogue entries.</returns>
    Task<PagedResponse<VideoGameResponse>> GetAllAsync(VideoGameBrowseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single catalogue entry by identifier.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The requested catalogue entry.</returns>
    Task<VideoGameResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new catalogue entry.
    /// </summary>
    /// <param name="request">The request payload used to create the catalogue entry.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The created catalogue entry.</returns>
    Task<VideoGameResponse> CreateAsync(SaveVideoGameRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing catalogue entry.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="request">The request payload containing the updated values.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The updated catalogue entry.</returns>
    Task<VideoGameResponse> UpdateAsync(int id, SaveVideoGameRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing catalogue entry.
    /// </summary>
    /// <param name="id">The catalogue entry identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
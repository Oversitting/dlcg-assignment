using VideoGameCatalogue.Api.Contracts;

namespace VideoGameCatalogue.Api.Implementation.Services;

public interface IVideoGameService
{
    Task<PagedResponse<VideoGameResponse>> GetAllAsync(VideoGameBrowseRequest request, CancellationToken cancellationToken = default);

    Task<VideoGameResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<VideoGameResponse> CreateAsync(SaveVideoGameRequest request, CancellationToken cancellationToken = default);

    Task<VideoGameResponse> UpdateAsync(int id, SaveVideoGameRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
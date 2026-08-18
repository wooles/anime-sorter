using MalProxy.Models;

namespace MalProxy.Services;

public interface IMyAnimeListService
{
    Task<IReadOnlyList<MalAnimeItem>> GetUserWatchlistAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MalAnimeItem>> SearchAnimeAsync(string query, CancellationToken cancellationToken = default);
}

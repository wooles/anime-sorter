namespace MalProxy.Models;

public record MalAnimeItem(
    long MalId,
    string Title,
    string? CoverUrl,
    string? Status,
    double? Score,
    int? EpisodesWatched = 0,
    int? TotalEpisodes = 0
);

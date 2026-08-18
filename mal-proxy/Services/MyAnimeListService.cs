using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MalProxy.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MalProxy.Services;

public class MyAnimeListService : IMyAnimeListService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MyAnimeListService> _logger;

    public MyAnimeListService(
        HttpClient httpClient,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<MyAnimeListService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MalAnimeItem>> GetUserWatchlistAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty", nameof(username));
        }

        var normalizedUser = username.Trim().ToLowerInvariant();
        var cacheKey = $""mal:user:{normalizedUser}:watching"";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<MalAnimeItem>? cachedList) && cachedList != null)
        {
            _logger.LogInformation(""Returning cached watchlist for user {Username}"", username);
            return cachedList;
        }

        var clientId = _configuration[""MyAnimeList:ClientId""] ?? Environment.GetEnvironmentVariable(""MAL_CLIENT_ID"");
        List<MalAnimeItem> results;

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            results = await FetchFromOfficialApiAsync(username, clientId, cancellationToken);
        }
        else
        {
            results = await FetchFromPublicJsonEndpointAsync(username, cancellationToken);
        }

        var cacheDurationMinutes = _configuration.GetValue(""CacheDurationMinutes"", 10);
        _cache.Set(cacheKey, results, TimeSpan.FromMinutes(cacheDurationMinutes));

        return results;
    }

    private async Task<List<MalAnimeItem>> FetchFromOfficialApiAsync(string username, string clientId, CancellationToken cancellationToken)
    {
        _logger.LogInformation(""Fetching MAL watchlist for user {Username} via official v2 API"", username);
        var url = $""https://api.myanimelist.net/v2/users/{Uri.EscapeDataString(username)}/animelist?status=watching&limit=500&fields=id,title,main_picture,status,mean,num_episodes,my_list_status"";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add(""X-MAL-CLIENT-ID"", clientId);
        request.Headers.UserAgent.ParseAdd(""sort-moe-proxy/1.0"");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(""Official MAL API returned status {StatusCode}: {ErrorBody}"", response.StatusCode, errorBody);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($""User '{username}' was not found on MyAnimeList."");
            }

            // Fallback to public web endpoint if official API fails (e.g. rate limit)
            return await FetchFromPublicJsonEndpointAsync(username, cancellationToken);
        }

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var jsonDoc = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);

        var list = new List<MalAnimeItem>();
        if (jsonDoc.RootElement.TryGetProperty(""data"", out var dataArray))
        {
            foreach (var item in dataArray.EnumerateArray())
            {
                if (item.TryGetProperty(""node"", out var node))
                {
                    var id = node.GetProperty(""id"").GetInt64();
                    var title = node.GetProperty(""title"").GetString() ?? """";
                    
                    string? coverUrl = null;
                    if (node.TryGetProperty(""main_picture"", out var pic))
                    {
                        if (pic.TryGetProperty(""large"", out var large))
                            coverUrl = large.GetString();
                        else if (pic.TryGetProperty(""medium"", out var medium))
                            coverUrl = medium.GetString();
                    }

                    string? status = node.TryGetProperty(""status"", out var st) ? st.GetString() : null;
                    double? score = node.TryGetProperty(""mean"", out var sc) && sc.ValueKind == JsonValueKind.Number ? sc.GetDouble() : null;
                    int? totalEps = node.TryGetProperty(""num_episodes"", out var eps) ? eps.GetInt32() : null;

                    int? watchedEps = null;
                    if (item.TryGetProperty(""list_status"", out var listStatus) && listStatus.TryGetProperty(""num_episodes_watched"", out var nw))
                    {
                        watchedEps = nw.GetInt32();
                    }

                    list.Add(new MalAnimeItem(id, title, coverUrl, status, score, watchedEps, totalEps));
                }
            }
        }

        return list;
    }

    private async Task<List<MalAnimeItem>> FetchFromPublicJsonEndpointAsync(string username, CancellationToken cancellationToken)
    {
        _logger.LogInformation(""Fetching MAL watchlist for user {Username} via public JSON endpoint"", username);
        var url = $""https://myanimelist.net/animelist/{Uri.EscapeDataString(username)}/load.json?status=1"";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36"");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($""User '{username}' was not found or list is private on MyAnimeList."");
            }
            throw new HttpRequestException($""Failed to fetch MAL watchlist for user '{username}'. Status: {response.StatusCode}"");
        }

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var jsonDoc = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);

        var list = new List<MalAnimeItem>();
        if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in jsonDoc.RootElement.EnumerateArray())
            {
                var id = item.GetProperty(""anime_id"").GetInt64();
                var title = item.GetProperty(""anime_title"").GetString() ?? """";
                var coverUrl = item.TryGetProperty(""anime_image_path"", out var img) ? img.GetString() : null;
                
                // Normalizing airing status
                var airingStatus = item.TryGetProperty(""anime_airing_status"", out var ast) ? ast.GetInt32() : 0;
                string status = airingStatus switch
                {
                    1 => ""currently_airing"",
                    2 => ""finished_airing"",
                    3 => ""not_yet_aired"",
                    _ => ""unknown""
                };

                double? score = item.TryGetProperty(""score"", out var sc) && sc.ValueKind == JsonValueKind.Number && sc.GetDouble() > 0 
                    ? sc.GetDouble() 
                    : null;

                int watchedEps = item.TryGetProperty(""num_watched_episodes"", out var we) ? we.GetInt32() : 0;
                int totalEps = item.TryGetProperty(""anime_num_episodes"", out var te) ? te.GetInt32() : 0;

                list.Add(new MalAnimeItem(id, title, coverUrl, status, score, watchedEps, totalEps));
            }
        }

        return list;
    }
}

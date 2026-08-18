# sort.moe MAL Proxy (.NET 8 Minimal API)

A lightweight proxy microservice for **sort.moe** that enables secure retrieval of user watchlists and search queries from **MyAnimeList (MAL)**.

## Key Features
* **CORS Enabled:** Allows requests from any frontend domain (`Access-Control-Allow-Origin: *`), ideal for static SPA hosting (GitHub Pages).
* **Dual Fetch Strategy:** 
  1. Supports the official MAL API v2 with `X-MAL-CLIENT-ID` authentication header.
  2. Automatic fallback to the public MyAnimeList watchlist endpoint.
* **In-Memory Caching:** Caches responses for 10–30 minutes, preventing rate-limiting (HTTP 429) issues from upstream MAL servers.
* **Tenrai.Net Integration:** Uses the modern Tenrai.Net library for high-speed anime search.
* **Standardized JSON Output:** Returns clean, structured anime objects tailored for sort.moe.

---

## Local Development (.NET 8 SDK)

```bash
cd mal-proxy
dotnet run
```
The application will start at: `http://localhost:5000` (or the port specified in environment).

---

## Docker Deployment

```bash
cd mal-proxy
docker build -t sortmoe-mal-proxy .
docker run -d -p 8080:8080 --name mal-proxy sortmoe-mal-proxy
```

---

## Configuration (`appsettings.json` or Environment Variables)

* `MyAnimeList:ClientId` (or environment variable `MAL_CLIENT_ID`): Optional Client ID from MyAnimeList Developer portal.
* `CacheDurationMinutes`: Cache TTL duration in minutes (default: `10`).

---

## API Endpoints

* `GET /health` — Health check endpoint:
  ```json
  { "status": "Healthy", "timestamp": "2026-08-19T00:00:00Z" }
  ```

* `GET /api/mal/watchlist/{username}` — Retrieves currently watching anime for the user:
  ```json
  [
    {
      "malId": 52991,
      "title": "Sousou no Frieren",
      "coverUrl": "https://cdn.myanimelist.net/images/anime/1015/138006l.jpg",
      "status": "currently_airing",
      "score": 9.14,
      "episodesWatched": 12,
      "totalEpisodes": 28
    }
  ]
  ```

* `GET /api/mal/search?q={query}` — Searches anime by keyword via Tenrai.Net.

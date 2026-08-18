# sort.moe MAL Proxy (.NET 8 Minimal API)

Lekki mikroserwis pośredniczący (proxy) dla aplikacji **sort.moe**, umożliwiający bezpieczne pobieranie watchlisty użytkowników z **MyAnimeList (MAL)** bez potrzeby ręcznego eksportu pliku XML.

## Główne funkcje
* **CORS Enabled:** Zezwala na zapytania z dowolnej domeny frontendowej (`Access-Control-Allow-Origin: *`), idealne dla statycznych SPA (GitHub Pages).
* **Dual Fetch Strategy:** 
  1. Obsługa oficjalnego MAL API v2 z autoryzacją `X-MAL-CLIENT-ID`.
  2. Automatyczny fallback do publicznego endpointu MyAnimeList.
* **In-Memory Caching:** Zapamiętuje wyniki zapytań na 10 minut, eliminując ryzyko przekroczenia limitów zapytań (Rate-Limit) na MAL.
* **Standaryzowany format JSON:** Zwraca tablicę obiektów anime bezpośrednio zoptymalizowaną pod sort.moe.

---

## Uruchomienie lokalne (.NET 8 SDK)

```bash
cd mal-proxy
dotnet run
```
Aplikacja uruchomi się pod adresem: `http://localhost:5000` (lub portem wskazanym w logach).

---

## Uruchomienie w Dockerze

```bash
cd mal-proxy
docker build -t sortmoe-mal-proxy .
docker run -d -p 8080:8080 --name mal-proxy sortmoe-mal-proxy
```

---

## Konfiguracja (`appsettings.json` lub zmienne środowiskowe)

* `MyAnimeList:ClientId` (lub zmienna środowiskowa `MAL_CLIENT_ID`): Opcjonalny Client ID z panelu deweloperskiego MAL API.
* `CacheDurationMinutes`: Czas pamięci podręcznej w minutach (domyślnie: `10`).

---

## Endpointy API

* `GET /health` — Status zdrowia serwisu:
  ```json
  { "status": "Healthy", "timestamp": "2026-08-18T21:14:00Z" }
  ```

* `GET /api/mal/watchlist/{username}` — Pobiera aktualnie oglądane anime użytkownika:
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

# PROJECT_CONTEXT.md — anime-sorter (sort.moe)

## 1. Project Overview
* **Name:** `anime-sorter`
* **Live URL:** [https://sort.moe/](https://sort.moe/)
* **Type:** Standalone, 100% client-side single-page application (SPA).
* **Purpose:** Allows users to rank and sort anime titles using an interactive pairwise comparison (merge sort) algorithm with support for ties, automatic metadata/artwork fetching, and image export.

---

## 2. Tech Stack & Architecture
* **Frontend:** Vanilla HTML5, CSS3 (CSS Variables for themes), Vanilla JavaScript (ES6+ / IIFE pattern).
* **Storage:** Browser `localStorage` (caching metadata/covers, persisting list items, language, and theme).
* **External Libraries (CDN):**
  * `html2canvas` (v1.4.1) — client-side PNG image rendering of the ranking board.
* **Architecture Principles:**
  * **Zero-backend:** All API calls and storage operations execute directly in the user's browser.
  * **Rate-limiting resilience:** Multi-threaded fetching (concurrency = 5) with rate-limiting safety.
  * **CORS handling:** Base64 image conversion with proxy fallbacks (`wsrv.nl`, Google proxy, etc.) for canvas exports.

---

## 3. Core Features & Capabilities
1. **Watchlist Import:**
   * **AniList:** Fetching currently watching anime via AniList GraphQL API by username.
   * **Kitsu:** Fetching currently watching anime via Kitsu JSON:API by username.
   * **MyAnimeList:** Fetching currently watching anime via MyAnimeList load.json endpoint with community scores.
   * **MyAnimeList XML:** Parsing `.xml` export files locally, filtering items with status `Watching` (`1`), extracting `series_title` and `series_animedb_id`.
2. **Manual Title Input:** Textarea supporting line breaks and semicolon-separated title lists.
3. **Triple-Source Metadata & Artwork Engine:**
   * **MAL ID Resolution:** When importing from MAL XML or API, directly resolves entries via AniList GraphQL `Media(idMal: $idMal)` before falling back to string queries.
   * **Triple Scores:** Simultaneously queries and displays community scores from MyAnimeList, AniList, and Kitsu (`MAL: ★ X.XX • AniList: ★ XX% • Kitsu: ★ XX%`).
   * **Status Badges:** Displays anime status badges (*Currently Airing*, *Finished*, *Upcoming / Not Yet Released*).
4. **Season Disambiguation Picker:** In-app modal search to switch franchise seasons, sequels, or alternate adaptations.
5. **Interactive Sorting Engine:**
   * Pairwise merge sort with progress tracking.
   * `🤝 Tie` button grouping equivalent titles into shared ranks.
6. **Export & Sharing:**
   * High-resolution PNG image download.
   * Instant temporary upload to Litterbox (Catbox.moe, 72h expiry) with 1-click URL copy.
7. **UI / UX:**
   * Bilingual localization (EN / PL) with persistent state.
   * Dark / Light mode with OS preference detection.

---

## 4. API Reference & Data Flow

### AniList GraphQL API (`https://graphql.anilist.co`)
* **Watchlist query:** `MediaListCollection(userName: $userName, type: ANIME, status: CURRENT)`
* **MAL ID lookup:** `Media(idMal: $idMal, type: ANIME)`
* **Text search query:** `Page(page: 1, perPage: 10) { media(search: $search, type: ANIME) }`

### Kitsu JSON:API (`https://kitsu.io/api/edge/`)
* **User ID lookup:** `users?filter[name]=...`
* **Library entries:** `library-entries?filter[userId]=...&filter[status]=current&include=anime`
* **Text search query:** `anime?filter[text]=...`

### MyAnimeList Search Prefix & Proxy (`https://myanimelist.net/search/prefix.json`)
* **Watchlist API:** `https://myanimelist.net/animelist/<username>/load.json?status=1` (with community `anime_score_val`)
* **Search Prefix:** `https://myanimelist.net/search/prefix.json?type=anime&keyword=<query>` (proxied locally via `/api/mal/search` and corsproxy fallback)

### Litterbox API (`https://litterbox.catbox.moe/resources/internals/api.php`)
* **Method:** `POST` (multipart form-data: `reqtype=fileupload`, `time=72h`, `fileToUpload=[blob]`).

---

## 5. Storage Schema (`localStorage`)
* `manual-anime-sorter:entries:v54` — JSON array of active entries (`id`, `name`, `malId`, `coverUrl`, `infoUrl`, `matchedTitle`, `source`, `malScore`, `anilistScore`, `kitsuScore`, `animeStatus`, `candidates`, `status`).
* `manual-anime-cover:v54:<normalized_title>` — cached metadata object.
* `manual-anime-sorter:theme:v1` — `'dark'` | `'light'`.
* `manual-anime-sorter:lang:v1` — `'en'` | `'pl'`.

---

## 6. Coding & Modification Rules for Agents
* **Workflow Trigger & Auto-Onboarding:** When the user says `"kontynuuj"`, `"kontynuuj anisort"`, or similar on any computer:
  1. Recognize the workspace as the `anime-sorter` (sort.moe) project.
  2. Inspect the local environment (check Git status/branches, verify if Python/.NET are available, run `setup.ps1` or restore dependencies if needed).
  3. Ensure recommended VS Code extensions/settings are available in `.vscode/`.
  4. Report to the user the current status of the project, git branch synchronization, and readiness to continue development.
* **GitHub Push Rule:** Always ask the user for explicit confirmation before executing `git push` to GitHub.
* **Complete Code Only:** Always maintain and deliver the entire `index.html` file without placeholders, truncation, or omission comments (`// ... rest of code`).
* **List Reset Behavior:** Any new import or manual add action must clear existing entries (`entries = []`) to prevent duplicate bloat.
* **No Server Dependencies for Client App:** The core application must run standalone on GitHub Pages / static hosting without requiring backend servers.
* **Concurrency & APIs:** Uses 12-thread parallel worker pool with Tenrai REST API (`api.tenrai.org`), AniList GraphQL, and Kitsu JSON:API.
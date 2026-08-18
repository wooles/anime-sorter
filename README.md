# anime-sorter

A web-based anime ranking and sorting tool. Allows users to run through a manual merge sort of their favorite or currently watching anime titles.

**Try it online:** [https://sort.moe/](https://sort.moe/)

## Features

* **Entirely client-side:** Runs completely in the browser as a standalone single-page application; no backend server required.
* **Watchlist Import:** One-click import of currently watching lists directly from **AniList**, **MyAnimeList**, or **Kitsu** by username.
* **Manual Input:** Ability to paste custom anime title lists.
* **Automatic Artwork & Triple Scores:** Fetches official anime cover images, release status badges (*Airing* / *Finished*), and community ratings from **MyAnimeList**, **AniList** & **Kitsu**.
* **Manual Merge Sort:** Interactive pairwise comparison algorithm with full support for ties.
* **Season Disambiguation Picker:** Search and switch between different seasons, sequels, or franchise entries on demand.
* **Export & Sharing:**
  * Download the finalized ranking as a high-resolution PNG image (powered by `html2canvas`).
  * Instant cloud upload to **Litterbox** with a direct, shareable temporary link (valid for 72 hours) and one-click clipboard copy.
* **Local Caching:** Persistent `localStorage` cache for anime metadata and covers to prevent redundant API calls.

## How to Use

1. **Add Anime Titles:**
   * **Import:** Select AniList or Kitsu, enter your username, and click `Import Watching`.
   * **Manual Entry:** Paste anime titles into the text area (one per line) and click `Add titles`.
2. **Review & Adjust:**
   * Wait for covers and scores to load automatically.
   * If a title matched the wrong season or adaptation, click `🔍 Season` to search and pick the correct one from the selection window.
3. **Sort:**
   * Click `Start sorting` to begin pairwise matchups.
   * Click on the anime you prefer, or click `🤝 Tie` if you consider them equally good.
4. **Export Results:**
   * View the final ranked list.
   * Click `📸 Download ranking as image (PNG)` to save your ranking locally.
   * Click `☁️ Upload to Litterbox (link)` to upload your ranking image and get a shareable link.

## API & Privacy

* All network requests are executed directly from your browser to public endpoints:
  * [AniList GraphQL API](https://anilist.gitbook.io/anilist-apiv2-docs/)
  * [Kitsu Edge API](https://kitsu.docs.apiary.io/)
  * [Litterbox (Catbox.moe)](https://litterbox.catbox.moe/)
* No user data, watchlists, or preferences are stored on any external server. All state is preserved locally in your browser's `localStorage`.

## Credits

* [charasort](https://github.com/execfera/charasort) — original concept & merge sort inspiration
* [html2canvas](https://html2canvas.hertzen.com/) — client-side image generation
* [Litterbox / Catbox.moe](https://catbox.moe/) — temporary image hosting
* [AniList](https://anilist.co/) & [Kitsu](https://kitsu.io/) — anime databases and artwork

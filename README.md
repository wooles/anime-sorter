# Anime Sorter (sort.moe)

A fast, lightweight, and client-side web application to sort, rank, and create tier lists of anime series through head-to-head comparisons. 

Built as a single-file application with zero backend requirements, automated metadata/cover retrieval from **AniList** and **Kitsu**, and one-click ranking export.

---

## Features

* **$O(n \log n)$ Merge Sort Engine:** Minimizes the number of 1v1 comparisons needed to generate an exact, mathematically consistent ranking. Includes dedicated **Tie** support for equal matchups.
* **Smart Cover & Metadata Fetching:**
  * Auto-fetches official artwork and community ratings from **AniList** (GraphQL) and **Kitsu API**.
  * **Airing Priority:** Automatically selects currently airing seasons (`RELEASING` / `CURRENT`) by default when generic titles are entered.
  * **Filters Unreleased Shows:** Automatically filters out upcoming/unreleased titles to prevent invalid matchups.
* **Season / Version Disambiguation:** Manual season selector modal (**🔍 Season**) with an integrated search tool to quickly switch between seasons, OVAs, or spin-offs.
* **Side-by-Side Community Ratings:** Displays verified community scores directly on final ranking cards (`AniList: ★ X% • Kitsu: ★ Y%`).
* **High-Res PNG Export & Image Sharing:**
  * **Export to PNG:** Renders high-resolution ranking cards locally via `html2canvas` with built-in Base64 cross-origin proxying to prevent CORS tainted canvas issues across all modern browsers.
  * **Instant Upload to Litterbox (Catbox):** 1-click cloud upload with 72h temporary hosting and automatic link copying to the clipboard.
* **Bilingual UI & Themes:** Instant toggle between English and Polish (`EN` / `PL`) and Dark / Light themes.
* **Zero Backend & Offline-Ready:** Entirely client-side with persistent `localStorage` caching.

---

## Live version

Try the live version at: **[https://sort.moe](https://sort.moe)**

---

## How to Run Locally

Because the application is completely self-contained in a single file, you do not need Node.js, Docker, or build tools:

1. Clone or download this repository:
   ```bash
   git clone [https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git)

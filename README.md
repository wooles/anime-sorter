# anime-sorter

A client-side pairwise anime and manga ranking tool. Allows users to run through an internal merge sort of their custom list of titles to produce a final ordered ranking.

### Features

* Entirely client-side, no backend server or database setup required.
* Merge sort algorithm ($O(n \log n)$) to minimize the number of user comparisons.
* Support for tie choices to allow shared ranking positions.
* Automatic cover art and community rating retrieval via AniList GraphQL and Kitsu APIs.
* High-resolution PNG image generation of the final ranking card.
* Direct image upload to Litterbox (Catbox.moe) with automatic clipboard copy.
* Dark / Light mode toggle with local storage persistence.
* Local storage cache for fetched artwork and query metadata.

### How to Use

1. Paste a list of anime or manga titles into the input area (one title per line).
2. Click **Pobierz okładki** to resolve covers and community scores.
3. Click **Rozpocznij porównywanie** and select preferred titles (or Ties) until the progress bar reaches 100%.
4. Review the final ranking and choose to either download the result as a `.png` file or upload it directly to Litterbox for sharing.

### Deployment

This project consists of a single standalone `index.html` file and requires no build pipeline.

To host on GitHub Pages:
1. Go to repository **Settings** -> **Pages**.
2. Under **Branch**, select `main` (or `master`) and folder `/ (root)`.
3. Click **Save**.

### Built With

* Vanilla HTML5 / CSS3 / JavaScript (ES6+)
* [html2canvas](https://html2canvas.hertzen.com/) — Canvas screenshot rendering.
* [AniList API](https://anilist.gitbook.io/anilist-apiv2-docs/) — GraphQL metadata and score queries.
* [Kitsu API](https://kitsu.docs.apiary.io/) — Fallback metadata queries.
* [Jikan API](https://jikan.moe/) — MyAnimeList search resolution.
* [Litterbox](https://litterbox.catbox.moe/) — Temporary image hosting.

### Credits

* [execfera/charasort](https://github.com/execfera/charasort) for the original sorter inspiration.

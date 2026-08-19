# 🏆 Anime Sorter (sort.moe)

[![Live Web App](https://img.shields.io/badge/Live-sort.moe-blue?style=for-the-badge&logo=google-chrome&logoColor=white)](https://sort.moe/)
[![Calendar Ecosystem](https://img.shields.io/badge/📅_Calendar-sort.moe/calendar-6c5ce7?style=for-the-badge)](https://sort.moe/calendar)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

A modern, fast, and feature-packed web application for interactive anime sorting, pairwise merge ranking, and seasonal schedule tracking.

👉 **Try it online:** **[https://sort.moe/](https://sort.moe/)**  
👉 **Anime Watching Calendar:** **[https://sort.moe/calendar](https://sort.moe/calendar)**

---

## ✨ Features

- ⚡ **100% Client-Side SPA**: Runs directly in the browser with lightning speed, zero tracking, and full privacy.
- 📥 **Watchlist Importer**:
  - **AniList**: Real-time GraphQL synchronization of currently watching anime.
  - **Kitsu**: Full JSON:API v3 library entry resolution.
  - **MyAnimeList**: Direct Tenrai.Net backend proxy or 1-click MAL XML export file import.
- ✍️ **Manual Title Input**: Paste custom lists of titles with multi-line or semicolon delimiters.
- 🎨 **Automated Artwork & Triple Community Scores**: Concurrently resolves high-resolution cover artwork, airing status badges (*Currently Airing*, *Finished*, *Upcoming*), and community ratings from **MyAnimeList**, **AniList**, and **Kitsu**.
- 🔀 **Pairwise Merge Sort Engine**: Interactive head-to-head comparison algorithm with progress tracking and full support for ties (`🤝 Tie`).
- 🔍 **Season Disambiguation Modal**: Easily switch between prequels, sequels, spin-offs, and alternate adaptations.
- 📅 **Ecosystem Integration**: Built-in 1-click navigation to the **Anime Watching Calendar** ([livechart-anime-tracker](https://github.com/wooles/livechart-anime-tracker)).
- 📸 **Export & Instant Share**:
  - Download high-resolution PNG ranking image cards.
  - 1-click cloud upload to Litterbox (Catbox.moe) with a 72-hour temporary link and auto-copy to clipboard.
- 🌐 **Localization & Dark/Light Themes**: Bilingual user interface (English & Polish) with persistent localStorage state and OS color scheme preference detection.

---

## 🛠️ Tech Stack

- **Frontend**: Vanilla HTML5, CSS3 (Modern CSS Variables, Flexbox/Grid), Vanilla ES6+ JavaScript.
- **Backend / Microservices**: .NET 8 Minimal API (`mal-proxy` / `Tenrai.Net 3.1.0`).
- **External APIs**: AniList GraphQL, Kitsu REST v3, Tenrai.Net REST, Litterbox API.
- **Third-Party Libraries**: `html2canvas` for client-side rasterization.

---

## 🚀 Development & Workflow

### Onboarding Command:
Whenever you start a session in any agent environment, you can type:
```text
kontynuuj anisort
```
This command automatically:
1. Clones/pulls both repositories (`wooles/anime-sorter` and `wooles/livechart-anime-tracker`).
2. Restores all .NET dependencies, Tenrai.Net packages, and configurations.
3. Sets up `.vscode` development environments and bindings.
4. Reports full workspace readiness.

### Quick Setup:
```powershell
# Run workspace setup script
.\setup.ps1

# Start local development server
python server.py
# Or open index.html directly in your browser
```

---

## 🔗 Related Projects

* 📅 **[wooles/livechart-anime-tracker](https://github.com/wooles/livechart-anime-tracker)** — Monthly anime watching calendar with exact LiveChart broadcasting schedules powered by .NET 8 + Tenrai.Net.

---

## 📄 License

Distributed under the [MIT License](LICENSE).

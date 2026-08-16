🎌 Anime Sorter / Anime Ranking
A lightweight, fully client-side web tool designed to create personalized anime and manga rankings through pairwise comparisons powered by the Merge Sort algorithm.

Inspired by classic character sorters (such as execfera/charasort), enhanced with automatic metadata retrieval, cover art fetching, community scores, and image export capabilities.

✨ Features
Merge Sort Algorithm — Minimizes the number of comparisons required to build an accurate ranking (O(nlogn)).

Tie Handling — Option to declare a tie between two titles and assign shared ranks.

Automatic Cover Art Fetching — Dynamic artwork matching with multi-source cascading fallbacks:

AniList (GraphQL API)

Kitsu (JSON API)

MyAnimeList (Jikan REST API)

Community Ratings — Displays average percentage scores from AniList and Kitsu next to the final ranking entries.

Visual Progress Bar — Real-time estimated sorting completion indicator.

Image Export (PNG) — Generates a high-resolution (2x scale) image snapshot of the completed ranking.

Instant Litterbox Upload — Upload the generated graphic directly to Litterbox (Catbox.moe) with automatic clipboard URL copying (72-hour retention).

Light & Dark Mode — Full theme support with preferences saved to localStorage.

Zero Backend Dependencies — The entire application is self-contained in a single index.html file.

Local Caching — Saved lists, ratings, and fetched covers persist locally in the browser's storage.

🚀 How to Use
Paste your list: Enter your anime or manga titles into the text area (one title per line).

Fetch covers: Click Pobierz okładki (Fetch Covers) to automatically pull artwork and ratings from AniList / Kitsu / MyAnimeList.

Start comparing: Click on the title you prefer, or select Remis (Tie) if you value both equally.

Save & Share: Once sorting is complete, you can:

Download the ranking as a .png file to your device.

Generate a direct link via Litterbox to share with others.

🛠️ Local Setup & Hosting
No Node.js, web server, or build step required.

Local:
Clone or download the repository (or copy the index.html file).

Open index.html in any modern web browser (Chrome, Brave, Firefox, Edge, Safari).

Hosting (GitHub Pages):
Navigate to Settings → Pages in your GitHub repository.

Under Branch, select main (or master) and click Save.

Your application will be live at:

https://<your-username>.github.io/<repository-name>/

📦 Built With & APIs Used
HTML5 / CSS3 / Vanilla JavaScript (ES6+)

html2canvas — Client-side DOM-to-Canvas / PNG rendering.

AniList GraphQL API — Anime and manga metadata and rating retrieval.

Kitsu API — Fallback database for covers and ratings.

Jikan API — Unofficial MyAnimeList REST API for search fallback.

Litterbox (Catbox.moe) — Temporary image hosting for exported ranking cards.

💡 Acknowledgements & Inspiration
execfera/charasort — The original inspiration for pairwise sorting workflows.

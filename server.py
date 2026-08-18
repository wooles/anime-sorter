import http.server
import json
import os
import re
import socketserver
import time
import urllib.parse
import urllib.request
import urllib.error

PORT = 5000
CACHE = {}
CACHE_SEARCH = {}
CACHE_TTL = 600

def clean_mal_image_url(url):
    if not url:
        return ""
    # Strip resize prefixes like /r/192x272 from MAL CDN URLs
    clean = re.sub(r'/r/\d+x\d+', '', url)
    # Strip query parameters (?s=...)
    clean = clean.split('?')[0]
    return clean

class SorterHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type, Authorization, X-MAL-CLIENT-ID')
        self.send_header('Cache-Control', 'no-cache, no-store, must-revalidate')
        super().end_headers()

    def do_OPTIONS(self):
        self.send_response(204)
        self.end_headers()

    def do_GET(self):
        if self.path == '/health':
            self.send_response(200)
            self.send_header('Content-Type', 'application/json')
            self.end_headers()
            self.wfile.write(json.dumps({"status": "Healthy", "timestamp": time.time()}).encode('utf-8'))
            return

        parsed = urllib.parse.urlparse(self.path)

        if parsed.path.startswith('/api/mal/search'):
            params = urllib.parse.parse_qs(parsed.query)
            q = params.get('q', [''])[0].strip()
            self.handle_mal_search(q)
            return

        match = re.match(r'^/api/mal/watchlist/([^/?#]+)', self.path)
        if match:
            raw_username = match.group(1)
            username = urllib.parse.unquote(raw_username).strip()
            self.handle_mal_watchlist(username)
            return

        super().do_GET()

    def handle_mal_search(self, query):
        if not query:
            self.send_json(400, {"error": "Query parameter 'q' cannot be empty"})
            return

        cache_key = query.lower()
        now = time.time()
        if cache_key in CACHE_SEARCH:
            cached_data, timestamp = CACHE_SEARCH[cache_key]
            if now - timestamp < CACHE_TTL:
                self.send_json(200, cached_data)
                return

        target_url = "https://myanimelist.net/search/prefix.json?type=anime&keyword=" + urllib.parse.quote(query)
        headers = {
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36",
            "Accept": "application/json"
        }

        try:
            req = urllib.request.Request(target_url, headers=headers)
            with urllib.request.urlopen(req, timeout=10) as response:
                if response.status != 200:
                    self.send_json(response.status, {"error": "MyAnimeList returned HTTP " + str(response.status)})
                    return
                raw_bytes = response.read()
                data = json.loads(raw_bytes.decode('utf-8'))

                formatted = []
                for cat in data.get('categories', []):
                    if cat.get('type') == 'anime':
                        for item in cat.get('items', []):
                            payload = item.get('payload', {})
                            raw_cover = item.get('image_url', '')
                            high_res_cover = clean_mal_image_url(raw_cover)
                            score = payload.get('score')
                            formatted.append({
                                "malId": item.get("id"),
                                "title": item.get("name", ""),
                                "coverUrl": high_res_cover,
                                "malScore": str(score) if score is not None and str(score).strip() else "",
                                "animeStatus": payload.get("status", ""),
                                "mediaType": payload.get("media_type", "")
                if formatted:
                    CACHE_SEARCH[cache_key] = (formatted, now)
                    self.send_json(200, formatted)
                    return
        except Exception:
            pass

        # Fallback to Tenrai REST API
        try:
            tenrai_url = "https://api.tenrai.org/v1/anime?q=" + urllib.parse.quote(query) + "&limit=5"
            req2 = urllib.request.Request(tenrai_url, headers={"User-Agent": "sort-moe"})
            with urllib.request.urlopen(req2, timeout=5) as resp:
                if resp.status == 200:
                    t_data = json.loads(resp.read().decode('utf-8'))
                    formatted = []
                    for item in t_data.get('data', []):
                        img = item.get('images', {}).get('jpg', {}).get('large_image_url', '')
                        formatted.append({
                            "malId": item.get("mal_id"),
                            "title": item.get("title", ""),
                            "coverUrl": img,
                            "malScore": str(item.get("score")) if item.get("score") else "",
                            "animeStatus": item.get("status", ""),
                            "mediaType": item.get("type", "")
                        })
                    if formatted:
                        CACHE_SEARCH[cache_key] = (formatted, now)
                        self.send_json(200, formatted)
                        return
        except Exception as ex:
            self.send_json(500, {"error": "MAL/Tenrai search error: " + str(ex)})
            return

        self.send_json(200, [])

    def handle_mal_watchlist(self, username):
        if not username:
            self.send_json(400, {"error": "Username cannot be empty"})
            return

        cache_key = username.lower()
        now = time.time()
        if cache_key in CACHE:
            cached_data, timestamp = CACHE[cache_key]
            if now - timestamp < CACHE_TTL:
                self.send_json(200, cached_data)
                return

        target_url = "https://myanimelist.net/animelist/" + urllib.parse.quote(username) + "/load.json?status=1"
        headers = {
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36",
            "Accept": "application/json"
        }

        try:
            req = urllib.request.Request(target_url, headers=headers)
            with urllib.request.urlopen(req, timeout=12) as response:
                if response.status != 200:
                    self.send_json(response.status, {"error": "MyAnimeList returned HTTP " + str(response.status)})
                    return
                raw_bytes = response.read()
                data = json.loads(raw_bytes.decode('utf-8'))

                formatted = []
                if isinstance(data, list):
                    for item in data:
                        airing_st = item.get("anime_airing_status", 0)
                        st_map = {1: "currently_airing", 2: "finished_airing", 3: "not_yet_aired"}
                        raw_cover = item.get("anime_image_path", "")
                        high_res_cover = clean_mal_image_url(raw_cover)
                        mal_comm_score = item.get("anime_score_val")
                        
                        formatted.append({
                            "malId": item.get("anime_id"),
                            "title": item.get("anime_title", ""),
                            "coverUrl": high_res_cover,
                            "status": st_map.get(airing_st, "unknown"),
                            "malScore": str(mal_comm_score) if mal_comm_score is not None and str(mal_comm_score).strip() else "",
                            "userScore": item.get("score") if item.get("score") and item.get("score") > 0 else None,
                            "episodesWatched": item.get("num_watched_episodes", 0),
                            "totalEpisodes": item.get("anime_num_episodes", 0)
                        })

                CACHE[cache_key] = (formatted, now)
                self.send_json(200, formatted)
        except urllib.error.HTTPError as e:
            if e.code == 404:
                self.send_json(404, {"error": "User '" + username + "' not found or profile list is private on MyAnimeList."})
            else:
                self.send_json(e.code, {"error": "MyAnimeList API error: " + str(e.reason)})
        except Exception as ex:
            self.send_json(500, {"error": "Internal proxy error: " + str(ex)})

    def send_json(self, status_code, payload):
        self.send_response(status_code)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        self.wfile.write(json.dumps(payload).encode('utf-8'))

if __name__ == '__main__':
    workspace_dir = os.path.dirname(os.path.abspath(__file__))
    os.chdir(workspace_dir)
    socketserver.TCPServer.allow_reuse_address = True
    with socketserver.TCPServer(("", PORT), SorterHandler) as httpd:
        print("sort.moe is running at http://localhost:" + str(PORT))
        print("MAL Proxy API available at http://localhost:" + str(PORT) + "/api/mal/watchlist/<username>")
        print("MAL Search API available at http://localhost:" + str(PORT) + "/api/mal/search?q=<query>")
        httpd.serve_forever()

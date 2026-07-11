# Adding a New Game

The full repo-setup checklist (repo, vault, labels, CI, branch protection) lives in `.claude/rules/adding-a-game.md` — Claude follows it automatically. This page covers the **portal-side wiring** and the contracts a game must honor.

---

## Game Contract

Games are **client-only** (static WASM/HTML behind their own nginx). No API server, no DB, no `/register`/`/login` — the portal owns auth and scores. See [[Tech/Architecture]].

The game's nginx proxies these paths to `portal-auth:5001`:

| Game path | Proxies to |
|-----------|-----------|
| `= /api/scores` | `/api/scores/{game-slug}` |
| `= /api/scores/me` | `/api/leaderboard/{game-slug}/me` |
| `/api/leaderboard` | `/api/leaderboard/{game-slug}` |

**sub_filter contract**: the game's `index.html` must contain the literal string `<base href="/"` — the portal nginx rewrites it to `<base href="/<game>/">` when serving the game under its path. Exact-string match; reformatting the tag silently breaks the game under the portal.

---

## Portal Wiring (one PR on game-portal)

**1. nginx location** in `nginx.conf` — copy the `/bh/` block, swap slug + container:

```nginx
location /<game>/ {
    proxy_pass         http://<game>-client:80/;   # trailing slash strips the prefix
    proxy_set_header   Accept-Encoding "";         # required for sub_filter
    sub_filter         '<base href="/"' '<base href="/<game>/"';
    sub_filter_once    on;
    proxy_set_header   Host              $host;
    proxy_set_header   X-Real-IP         $remote_addr;
    proxy_set_header   X-Forwarded-For   $proxy_add_x_forwarded_for;
}
```

**2. compose service** in `docker-compose.yml`:

```yaml
<game>-client:
  image: ghcr.io/alon-shviki/<game>-client:latest
  ports:
    - "<dev-port>:80"   # dev-only direct access — portal serves the game at /<game>/
  depends_on:
    - portal-auth
```

**3. shell card** in `shell/index.html` — copy an existing game card, point its button at `openGame('/<game>', '<Game Name>')`.

**4. leaderboard tab** in `shell/index.html` if the game submits scores.

---

## Vault Wiring

- Symlink the game's notes: `ln -s ~/Desktop/<game>/Notes ~/Desktop/game/Games/<game>`
- Add `Games/<Game Name>.md` hub page, update `Home.md` game table

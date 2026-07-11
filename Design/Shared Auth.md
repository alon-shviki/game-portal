# Shared Authentication

## Goal

One username + password works across every game in the portal. Log in once on the portal, play any game without logging in again.

---

## How It Works

```
[Portal Shell]  ← user logs in here (localhost:3000)
     │
     │  POST /api/auth/login → { token }
     │  token stored in localStorage["jwt"]
     │
     │  Play button clicked → game opens with #portal_token=<jwt> in URL hash
     ↓
[Game client]   ← reads hash on load, stores to localStorage["jwt"]
     │
     │  reads localStorage["jwt"] on startup
     │  Authorization: Bearer <token>  on all API calls
     ↓
[Portal Auth Server]  ← validates JWT, records scores
```

---

## JWT Claims

```json
{
  "sub": "42",
  "unique_name": "alon",
  "exp": 1750086400
}
```

- `sub` — user ID (integer)
- `unique_name` — display name (used on leaderboards, matches `JwtRegisteredClaimNames.UniqueName`)
- Token lifetime: 24 hours
- `MapInboundClaims = false` required on all JWT validators — keeps claim names as-is

---

## Auth Endpoints (Portal Auth Server)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/register` | Create account. 409 if username taken. |
| POST | `/api/login` | Returns signed JWT. 401 on bad creds. |
| GET | `/api/me` | Returns `{ id, username }` for current token. |
| POST | `/api/scores/{game}` | Submit a run score. Bearer required. |
| GET | `/api/leaderboard/{game}` | Top 10 for a game. Public. |
| GET | `/api/leaderboard/{game}/me` | User's top 5 runs for a game. Bearer required. |

Games expose **no** `/register` or `/login`. They only call portal endpoints (proxied via their own nginx).

---

## Cross-Origin Token Relay

Portal (port 3000) and games (port 8080) are different origins — `localStorage` is not shared. When the user clicks Play:

1. Portal's Play button: `window.open(gameUrl + '#portal_token=' + encodeURIComponent(jwt))`
2. Game's `index.html` reads the hash fragment before Blazor loads, stores to `localStorage["jwt"]`
3. Hash is cleared from the URL with `history.replaceState`

This is a dev-only concern. In production all services share the same origin.

---

## Security Rules

- Passwords: bcrypt hash only (`BCrypt.Net-Next`), explicit work factor 12, never logged or returned. Old hashes at other costs keep verifying — the cost is embedded per-hash.
- JWT signing key: one shared secret in `.env` — never hardcoded
- JWT validation: issuer, audience, lifetime, and signing key all explicitly validated (`Program.cs`); `MapInboundClaims = false` is load-bearing — endpoints read raw `sub`/`unique_name`
- Rate limiting: `/api/login` + `/api/register` capped at 10 requests/min per client IP (ASP.NET fixed-window limiter, partitioned by the last `X-Forwarded-For` entry — the one our own nginx appends; earlier entries are client-spoofable). Ceiling: in-memory, single instance — switch to nginx `limit_req` if the topology ever grows.
- HTTPS in production (nginx terminates TLS)
- Token in URL hash is not sent to server (fragment never leaves the browser)

## Refresh Tokens — Deliberately Skipped (July 2026)

The 30-day access token stays. A refresh flow means a new table, two endpoints, and client logic in three codebases — to protect a hobby leaderboard whose token already lives in `localStorage`. If revocation is ever needed, the upgrade path is a `RefreshTokens` table + `/api/refresh`; nothing else in the stack needs changing today.

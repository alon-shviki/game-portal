# Shared Authentication

## Goal

One username + password works across every game in the portal. Log in once on the portal, play any game without logging in again.

---

## How It Works

```
Portal Auth Server  (issues JWTs)
        ↑
        │  POST /api/login → { token }
        │
   [Portal Shell]  ← user logs in here
        │
        │  token stored in localStorage
        │
   [Game client]   ← reads token from localStorage on load
        │
        │  Authorization: Bearer <token>
        ↓
   [Game Server]   ← validates JWT, trusts claims
```

The JWT signing key is **shared** between the portal auth server and every game server. This means a token issued by the portal is accepted by any game without a round-trip back to the portal.

---

## JWT Claims

```json
{
  "sub": "42",
  "username": "alon",
  "iat": 1750000000,
  "exp": 1750086400
}
```

- `sub` — user ID (integer, unique across the platform)
- `username` — display name (used on leaderboards)
- Token lifetime: 24 hours (configurable)

---

## Auth Endpoints (Portal Auth Server)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/register` | Create account. 409 if username taken. |
| POST | `/api/login` | Returns signed JWT. 401 on bad creds. |
| GET | `/api/me` | Returns `{ id, username }` for current token. |

Game servers expose **no** `/register` or `/login` — they only accept and validate tokens.

---

## Token Storage

- Stored in `localStorage` under key `"jwt"` by the portal shell
- Game clients read it with `localStorage.getItem("jwt")` on startup
- Games attach it as `Authorization: Bearer <token>` on every API call
- On expiry: game client catches 401, redirects user back to portal login

---

## Security Rules

- Passwords: bcrypt hash only (`BCrypt.Net-Next`), never logged or returned
- JWT signing key: one shared secret, stored in environment variables / `dotnet user-secrets` on each server — never hardcoded
- HTTPS everywhere in production (nginx terminates TLS)
- Refresh tokens: not implemented yet — re-login on expiry

---

## Current State

Bullet Heaven has its own auth (`AuthController.cs`) that is a **standalone implementation** — it has its own user table and issues its own JWTs. When the portal auth server is built, the plan is:

1. Migrate Bullet Heaven's `User` table to the portal auth DB
2. Remove `AuthController` from `BulletHeaven.Server`
3. Game server trusts the shared JWT signing key instead

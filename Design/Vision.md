# Portal Vision

## What It Is

A single web destination that houses multiple original games. Each game is a fully independent full-stack project (its own repo, server, database) but they're presented as one cohesive platform.

The portal's job: **one URL, all games, one login**.

---

## Landing Page

The portal homepage is a game selection screen — like a Netflix row but for playable browser games.

### Layout Concept

```
┌─────────────────────────────────────────────┐
│  [LOGO]   My Games                [Login]   │
├─────────────────────────────────────────────┤
│                                             │
│   ┌──────────────┐   ┌──────────────┐      │
│   │              │   │              │      │
│   │  Bullet      │   │  Game 2      │      │
│   │  Heaven  →   │   │  (coming     │      │
│   │              │   │   soon)      │      │
│   └──────────────┘   └──────────────┘      │
│                                             │
│   ┌──────────────┐                         │
│   │  + Add game  │                         │
│   └──────────────┘                         │
│                                             │
└─────────────────────────────────────────────┘
```

### Game Card

Each game gets a card with:
- Title + short tagline
- Preview image / animated screenshot
- High score (if logged in: personal best + global rank)
- "Play" button → navigates to that game's URL

---

## User Journey

```
Land on portal
  ↓
Browse game cards (no login needed)
  ↓
Click "Play" on a game
  ↓
If not logged in → Login/Register overlay (shared auth)
  ↓
Game loads in the same tab (or new tab, TBD)
  ↓
Score submitted on game over → leaderboard updates
  ↓
Return to portal → personal best updated on card
```

---

## Key Design Decisions

| Decision | Choice | Why |
|----------|--------|-----|
| Game hosting | Each game on its own subdomain or path | Keeps codebases fully independent |
| Auth | Shared JWT, issued by a portal auth server | One account across all games |
| Leaderboard | Per-game endpoint, portal aggregates | Games stay self-contained |
| Routing | nginx reverse-proxy | `/bullet-heaven/*` → BulletHeaven.Server, `/` → portal shell |
| Tech | TBD — could be simple HTML/CSS or Blazor WASM | Depends on how interactive the shell needs to be |

---

## Future Ideas

- Global profile page: all-time stats across every game
- Achievements system: cross-game milestones
- Friend list / head-to-head leaderboard view
- Portal-level daily challenge: same seed in all games, compete for 24h

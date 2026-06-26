# UI & Visual Identity

## Theme

**Dark, high-contrast, game-feel.** The portal should feel like a game launcher — not a corporate SaaS dashboard.

---

## Colour Palette

| Role | Hex | Usage |
|------|-----|-------|
| Background | `#0d0d0f` | Page background, card base |
| Surface | `#1a1a1f` | Cards, modals, sidebars |
| Border | `#2a2a32` | Card outlines, dividers |
| Accent | `#7c3aed` | Primary buttons, highlights, active states |
| Accent hover | `#6d28d9` | Button hover |
| Text primary | `#f4f4f5` | Headings, labels |
| Text muted | `#71717a` | Subtitles, metadata |
| Success | `#22c55e` | "Play", positive states |
| Danger | `#ef4444` | Errors, health bars |
| XP / Gold | `#eab308` | Score highlights, XP indicators |

---

## Typography

- **Headings:** `Space Grotesk` or `Outfit` — geometric, slightly techy
- **Body / UI:** `Inter` — clean, legible at small sizes
- **Scores / Numbers:** `JetBrains Mono` or `Roboto Mono` — monospace so digits don't jump

---

## Game Card Component

```
┌──────────────────────────────┐
│  ░░░░░░░░░░░░░░░░░░░░░░░░░  │  ← preview image / gif
│  ░░░░░░░░░░░░░░░░░░░░░░░░░  │
│  ░░░░░░░░░░░░░░░░░░░░░░░░░  │
├──────────────────────────────┤
│  Bullet Heaven               │  ← title (Space Grotesk, 18px)
│  Survive. Level up. Repeat.  │  ← tagline (muted, 13px)
│                              │
│  🏆 Best: 14,200  #3 global  │  ← personal best + rank (if logged in)
│                              │
│  [        Play  →        ]   │  ← accent button
└──────────────────────────────┘
```

- Card background: `#1a1a1f`
- Card border: `1px solid #2a2a32`
- Hover: border glows accent colour (`box-shadow: 0 0 0 1px #7c3aed`)
- Corner radius: `12px`

---

## Shared UI Components

Components reused across the portal and games:

| Component | Used In |
|-----------|---------|
| Login / Register modal | Portal shell, game overlays |
| Leaderboard table | Portal profile, game game-over screen |
| Score badge | Game cards, HUD |
| Notification toast | Score saved, login error |

---

## Animations

- Page load: cards fade-in staggered (50ms delay per card)
- Card hover: subtle `translateY(-2px)` + border glow
- Play button: pulse glow on hover
- Keep motion minimal — the games themselves provide the excitement

---

## "Coming Soon" Card

Placeholder card for games not yet playable:

- Same card shape, image is blurred / greyed
- "Coming Soon" pill instead of Play button
- Still shows in the grid — signals the portal is actively growing

# Adding a New Game

Steps to add a new game to the portal.

---

## 1. Create the Game Project

```bash
mkdir ~/Desktop/<game-name>
cd ~/Desktop/<game-name>
# scaffold however fits the game's stack
```

Create an Obsidian vault:
```bash
mkdir .obsidian
echo '{}' > .obsidian/app.json
```

---

## 2. Wire Up Auth

The game server should **not** have its own user table or `/register`/`/login` endpoints.

Instead:
- Add JWT Bearer auth in the game server's `Program.cs`
- Set the signing key to the same `JWT_KEY` as the portal auth server
- All protected endpoints use `[Authorize]` — the token comes from the portal

```csharp
// Game server Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new() {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer   = false,
            ValidateAudience = false,
        };
    });
```

---

## 3. Add Score + Leaderboard Endpoints

Every game needs these two endpoints (same pattern as Bullet Heaven):

| Method | Path | Auth | Returns |
|--------|------|------|---------|
| POST | `/api/scores` | Bearer | Save a run score |
| GET | `/api/leaderboard` | Public | Top 10 `{ username, score }` |

---

## 4. Add to Portal nginx Config

```nginx
location /api/<game-name>/ {
    proxy_pass http://<game-name>-server:5000/api/;
}
```

---

## 5. Add to Portal Docker Compose

```yaml
<game-name>-server:
  build: ../<game-name>/<GameName>.Server
  environment:
    - JWT_KEY=${JWT_KEY}
    - DB_CONNECTION=${<GAME>_DB}
```

---

## 6. Add Game Card to Portal Home

In the portal shell's game list, add a new entry:

```
title:    "<Game Name>"
tagline:  "<one-line description>"
url:      "/<game-name>"
image:    "/assets/<game-name>-preview.gif"
status:   "live"   # or "coming-soon"
```

---

## 7. Add Notes to Portal Vault

- Add `Games/<Game Name>.md` to this vault with overview, stack, status
- Update `Home.md` game table
- Update `Roadmap.md` status

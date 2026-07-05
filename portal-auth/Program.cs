using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalAuth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.MapInboundClaims = false;  // keep JWT claim names as-is (sub, unique_name)
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidIssuer              = "Portal",
            ValidAudience            = "Portal",
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();

// ── Endpoints ────────────────────────────────────────────────────────────────

app.MapPost("/api/register", async (AuthRequest req, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) || req.Username.Length > 32)
        return Results.BadRequest("Username must be 1-32 characters.");

    if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
        return Results.BadRequest("Password must be at least 6 characters.");

    if (await db.Users.AnyAsync(u => u.Username == req.Username))
        return Results.Conflict("Username already taken.");

    var user = new User
    {
        Username     = req.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
        CreatedAt    = DateTime.UtcNow,
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new AuthResponse(BuildToken(user, jwtKey)));
});

app.MapPost("/api/login", async (AuthRequest req, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(new AuthResponse(BuildToken(user, jwtKey)));
});

app.MapGet("/api/me", (ClaimsPrincipal principal, AppDbContext db) =>
{
    var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var username = principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
    if (sub is null || username is null) return Results.Unauthorized();

    return Results.Ok(new MeResponse(int.Parse(sub), username));
}).RequireAuthorization();

app.MapGet("/health", () => Results.Ok("ok"));

// ── Scores & Leaderboard ─────────────────────────────────────────────────────

app.MapPost("/api/scores/{game}", async (string game, ScoreSubmitRequest req, ClaimsPrincipal principal, AppDbContext db) =>
{
    var sub      = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var username = principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
    if (sub is null || username is null) return Results.Unauthorized();

    if (req.Value < 0 || req.Kills < 0 || req.Level < 1) return Results.BadRequest("Invalid score data.");

    db.Scores.Add(new Score
    {
        UserId   = int.Parse(sub),
        Username = username,
        Game     = game,
        Value    = req.Value,
        Kills    = req.Kills,
        Level    = req.Level,
        PlayedAt = DateTime.UtcNow,
    });
    await db.SaveChangesAsync();
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/leaderboard/{game}", async (string game, AppDbContext db) =>
{
    var entries = await db.Scores
        .Where(s => s.Game == game)
        .OrderByDescending(s => s.Value)
        .Take(10)
        .Select(s => new LeaderboardEntry(s.Username, s.Value, s.Kills, s.Level, s.PlayedAt))
        .ToListAsync();
    return Results.Ok(entries);
});

app.MapGet("/api/leaderboard/{game}/me", async (string game, ClaimsPrincipal principal, AppDbContext db) =>
{
    var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (sub is null) return Results.Unauthorized();

    var entries = await db.Scores
        .Where(s => s.Game == game && s.UserId == int.Parse(sub))
        .OrderByDescending(s => s.Value)
        .Take(5)
        .Select(s => new PersonalBestEntry(s.Value, s.Kills, s.Level, s.PlayedAt))
        .ToListAsync();
    return Results.Ok(entries);
}).RequireAuthorization();

app.Run();

// ── Helpers ──────────────────────────────────────────────────────────────────

static string BuildToken(User user, string jwtKey)
{
    var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var exp = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds(); // long-lived so a docker restart doesn't force re-login
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim(JwtRegisteredClaimNames.Exp,        exp.ToString(), ClaimValueTypes.Integer64),
    };
    var token = new JwtSecurityToken(
        issuer:            "Portal",
        audience:          "Portal",
        claims:            claims,
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// ── Types ────────────────────────────────────────────────────────────────────

record AuthRequest(string Username, string Password);
record AuthResponse(string Token);
record MeResponse(int Id, string Username);
record ScoreSubmitRequest(int Value, int Kills, int Level);
record LeaderboardEntry(string Username, int Value, int Kills, int Level, DateTime PlayedAt);
record PersonalBestEntry(int Value, int Kills, int Level, DateTime PlayedAt);

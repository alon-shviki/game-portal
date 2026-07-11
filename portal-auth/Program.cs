using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidIssuer = "Portal",
            ValidAudience = "Portal",
            // Explicit — these are the defaults, but the security posture should be greppable.
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

// Brute-force guard on login/register. Partition by client IP: the LAST
// X-Forwarded-For entry is the one appended by our own nginx (earlier entries
// are client-supplied and spoofable); direct connections fall back to the socket IP.
builder.Services.AddRateLimiter(opt =>
{
    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opt.AddPolicy("auth", ctx =>
    {
        var forwarded = ctx.Request.Headers["X-Forwarded-For"].ToString();
        var ip = forwarded.Length > 0
            ? forwarded.Split(',')[^1].Trim()
            : ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
        });
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

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
        Username = req.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12),
        CreatedAt = DateTime.UtcNow,
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new AuthResponse(TokenService.CreateToken(user, jwtKey)));
}).RequireRateLimiting("auth");

app.MapPost("/api/login", async (AuthRequest req, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(new AuthResponse(TokenService.CreateToken(user, jwtKey)));
}).RequireRateLimiting("auth");

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
    var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var username = principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
    if (sub is null || username is null) return Results.Unauthorized();

    if (req.Value < 0 || req.Kills < 0 || req.Level < 1) return Results.BadRequest("Invalid score data.");

    db.Scores.Add(new Score
    {
        UserId = int.Parse(sub),
        Username = username,
        Game = game,
        Value = req.Value,
        Kills = req.Kills,
        Level = req.Level,
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

// ── Types ────────────────────────────────────────────────────────────────────

record AuthRequest(string Username, string Password);
record AuthResponse(string Token);
record MeResponse(int Id, string Username);
record ScoreSubmitRequest(int Value, int Kills, int Level);
record LeaderboardEntry(string Username, int Value, int Kills, int Level, DateTime PlayedAt);
record PersonalBestEntry(int Value, int Kills, int Level, DateTime PlayedAt);

// Exposes the entry point to WebApplicationFactory in PortalAuth.Tests.
public partial class Program { }

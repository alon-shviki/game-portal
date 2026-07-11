using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

// Spins up one real postgres container for the whole class — migrations run
// unmodified on startup, exactly like production.
public sealed class ApiFixture : IAsyncLifetime
{
    public const string JwtKey = "integration-test-signing-key-32-chars!!";

    readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .Build();

    WebApplicationFactory<Program>? _factory;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseSetting("ConnectionStrings:Default", _postgres.GetConnectionString());
            b.UseSetting("Jwt:Key", JwtKey);
        });
        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_factory is not null) await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}

public class ApiTests(ApiFixture fx) : IClassFixture<ApiFixture>
{
    record AuthResponse(string Token);
    record MeResponse(int Id, string Username);
    record LeaderboardEntry(string Username, int Value, int Kills, int Level, DateTime PlayedAt);
    record PersonalBestEntry(int Value, int Kills, int Level, DateTime PlayedAt);

    static int _ipCounter;

    // Each call gets its own X-Forwarded-For so the auth rate limiter (partitioned
    // by client IP) never throttles unrelated tests sharing the fixture.
    static string NextIp() => $"10.0.0.{Interlocked.Increment(ref _ipCounter)}";

    async Task<HttpResponseMessage> PostAuthAsync(string url, object body, string? ip = null)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body) };
        req.Headers.Add("X-Forwarded-For", ip ?? NextIp());
        return await fx.Client.SendAsync(req);
    }

    async Task<string> RegisterAsync(string username)
    {
        var resp = await PostAuthAsync("/api/register", new { username, password = "secret123" });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<AuthResponse>())!.Token;
    }

    static HttpRequestMessage Authed(HttpMethod method, string url, string token, object? body = null)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) req.Content = JsonContent.Create(body);
        return req;
    }

    [Fact]
    public async Task Register_ReturnsToken()
    {
        var token = await RegisterAsync("reg_user");
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Register_DuplicateUsername_Conflicts()
    {
        await RegisterAsync("dup_user");
        var resp = await PostAuthAsync("/api/register", new { username = "dup_user", password = "secret123" });
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_BadRequest()
    {
        var resp = await PostAuthAsync("/api/register", new { username = "shortpw_user", password = "12345" });
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_Unauthorized()
    {
        await RegisterAsync("login_user");
        var resp = await PostAuthAsync("/api/login", new { username = "login_user", password = "wrong-pass" });
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Login_ThenMe_RoundTrips()
    {
        await RegisterAsync("me_user");
        var login = await PostAuthAsync("/api/login", new { username = "me_user", password = "secret123" });
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<AuthResponse>())!.Token;

        var resp = await fx.Client.SendAsync(Authed(HttpMethod.Get, "/api/me", token));
        resp.EnsureSuccessStatusCode();
        var me = await resp.Content.ReadFromJsonAsync<MeResponse>();
        Assert.Equal("me_user", me!.Username);
    }

    [Fact]
    public async Task Me_WithoutToken_Unauthorized()
    {
        var resp = await fx.Client.GetAsync("/api/me");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Scores_SubmitWithoutToken_Unauthorized()
    {
        var resp = await fx.Client.PostAsJsonAsync("/api/scores/testgame", new { value = 1, kills = 0, level = 1 });
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Scores_InvalidLevel_BadRequest()
    {
        var token = await RegisterAsync("badscore_user");
        var resp = await fx.Client.SendAsync(
            Authed(HttpMethod.Post, "/api/scores/testgame", token, new { value = 10, kills = 1, level = 0 }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Scores_SubmitAndLeaderboard_RoundTrips()
    {
        // Own slug so other tests' scores can't interfere.
        const string game = "lb_game";
        var tokenA = await RegisterAsync("lb_alice");
        var tokenB = await RegisterAsync("lb_bob");

        (await fx.Client.SendAsync(
            Authed(HttpMethod.Post, $"/api/scores/{game}", tokenA, new { value = 100, kills = 5, level = 2 }))).EnsureSuccessStatusCode();
        (await fx.Client.SendAsync(
            Authed(HttpMethod.Post, $"/api/scores/{game}", tokenB, new { value = 300, kills = 9, level = 3 }))).EnsureSuccessStatusCode();

        var board = await fx.Client.GetFromJsonAsync<List<LeaderboardEntry>>($"/api/leaderboard/{game}");
        Assert.Equal(2, board!.Count);
        Assert.Equal("lb_bob", board[0].Username);   // ordered by value desc
        Assert.Equal("lb_alice", board[1].Username);

        var mine = await fx.Client.SendAsync(Authed(HttpMethod.Get, $"/api/leaderboard/{game}/me", tokenA));
        mine.EnsureSuccessStatusCode();
        var best = await mine.Content.ReadFromJsonAsync<List<PersonalBestEntry>>();
        Assert.Single(best!);
        Assert.Equal(100, best![0].Value);
    }

    [Fact]
    public async Task Login_Hammered_Returns429()
    {
        const string ip = "10.99.99.99"; // dedicated partition for this test
        HttpResponseMessage last = null!;
        for (var i = 0; i < 11; i++)
            last = await PostAuthAsync("/api/login", new { username = "nobody", password = "wrong" }, ip);

        Assert.Equal(HttpStatusCode.TooManyRequests, last.StatusCode);
    }

    [Fact]
    public async Task Leaderboard_IsScopedPerGame()
    {
        var token = await RegisterAsync("scope_user");
        (await fx.Client.SendAsync(
            Authed(HttpMethod.Post, "/api/scores/game_a", token, new { value = 42, kills = 1, level = 1 }))).EnsureSuccessStatusCode();

        var other = await fx.Client.GetFromJsonAsync<List<LeaderboardEntry>>("/api/leaderboard/game_b_empty");
        Assert.Empty(other!);
    }
}

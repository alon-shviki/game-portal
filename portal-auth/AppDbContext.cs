using Microsoft.EntityFrameworkCore;

namespace PortalAuth;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class Score
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = "";   // denormalised — no join on leaderboard read
    public string Game { get; set; } = "";
    public int Value { get; set; }
    public int Kills { get; set; }
    public int Level { get; set; }
    public DateTime PlayedAt { get; set; }
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Score> Scores => Set<Score>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.Username).IsUnique();
        b.Entity<Score>().HasIndex(s => new { s.Game, s.Value });   // leaderboard query
    }
}

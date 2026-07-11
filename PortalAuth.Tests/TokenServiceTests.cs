using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PortalAuth;

public class TokenServiceTests
{
    const string Key = "test-signing-key-at-least-32-chars-long!!";

    static JwtSecurityToken Decode(string jwt) => new JwtSecurityTokenHandler().ReadJwtToken(jwt);

    [Fact]
    public void CreateToken_EmitsRawJwtClaimNames()
    {
        // Guards the MapInboundClaims=false contract: claims must stay 'sub'/'unique_name'.
        var token = Decode(TokenService.CreateToken(new User { Id = 42, Username = "alice" }, Key));

        Assert.Equal("42", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("alice", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal("Portal", token.Issuer);
        Assert.Contains("Portal", token.Audiences);
    }

    [Fact]
    public void CreateToken_ExpiresIn30Days()
    {
        var token = Decode(TokenService.CreateToken(new User { Id = 1, Username = "bob" }, Key));

        var days = (token.ValidTo - DateTime.UtcNow).TotalDays;
        Assert.InRange(days, 29.9, 30.1);
    }

    [Fact]
    public void CreateToken_ValidatesAgainstSigningKey()
    {
        var jwt = TokenService.CreateToken(new User { Id = 7, Username = "carol" }, Key);

        var parms = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            ValidIssuer = "Portal",
            ValidAudience = "Portal",
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(jwt, parms, out var validated);

        Assert.Equal(SecurityAlgorithms.HmacSha256, ((JwtSecurityToken)validated).Header.Alg);
        Assert.NotNull(principal);
    }
}

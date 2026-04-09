using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BanSach.Auth;
using BanSach.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BanSach.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var normalizedUsername = request.Username.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await _context.Users.AnyAsync(u =>
            u.Username == normalizedUsername || u.Email == normalizedEmail);

        if (exists)
        {
            return null;
        }

        var (hash, salt) = PasswordHelper.HashPassword(request.Password);

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var key = request.UsernameOrEmail.Trim();
        var normalizedEmail = key.ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Username == key || u.Email == normalizedEmail);

        if (user == null)
        {
            return null;
        }

        var valid = PasswordHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!valid)
        {
            return null;
        }

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key in configuration.");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer in configuration.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience in configuration.");
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 120;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using System.ComponentModel.DataAnnotations;

namespace BanSach.Auth;

public class User
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(50)]
	public required string Username { get; set; }

	[Required]
	[MaxLength(120)]
	[EmailAddress]
	public required string Email { get; set; }

	[Required]
	public required string PasswordHash { get; set; }

	[Required]
	public required string PasswordSalt { get; set; }

	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

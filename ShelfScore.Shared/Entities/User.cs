namespace ShelfScore.Shared.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateTimeJoined { get; } = DateTime.UtcNow;

    public ICollection<Rating> Ratings { get; set; } = [];
}
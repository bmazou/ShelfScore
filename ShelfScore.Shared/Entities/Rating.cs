namespace ShelfScore.Shared.Entities;

public class Rating
{
    public int Id { get; set; }
    public required int Score { get; set; }
    public string? TextReview { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public required int UserId { get; set; }
    public required User User;

    public required int BookId { get; set; }
    public required Book Book; 
}
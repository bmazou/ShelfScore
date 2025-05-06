namespace ShelfScore.Shared.Dtos;


public record CreateRatingDto(
    int Score,
    string? TextReview,
    int BookId
    // UserId will get inferred from authentication
);
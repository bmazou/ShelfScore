namespace ShelfScore.Shared.Dtos;


public record RatingDto(
    int Id,
    int Score,
    string? TextReview,
    DateTime CreatedAt,
    UserSummaryDto User,
    RatedBookSummaryDto Book
);